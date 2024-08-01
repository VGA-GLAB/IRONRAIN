using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    // 一時的に画面から消して動作を止めたい場合: xxx
    // これ以上動かさず、削除する場合: SetActive(false)

    /// <summary>
    /// 敵を操作する。
    /// </summary>
    [RequireComponent(typeof(EnemyParams))]
    public class EnemyController : Character, IDamageable
    {
        [SerializeField] private Transform[] _models;
        [SerializeField] private EnemyEffects _effects;
        [SerializeField] private Collider[] _hitBoxes;

        private EnemyParams _params;
        // 自身の状態や周囲を認識して黒板に書き込むPerception層。
        private Perception _perception;
        private FireRate _fireRate;
        private EyeSensor _eyeSensor;
        private HitPoint _hitPoint;
        private OverrideOrder _overrideOrder;
        // 黒板に書き込まれた内容から次にとるべき行動を決定するBrain層。
        private UtilityEvaluator _utilityEvaluator;
        private BehaviorTree _behaviorTree;
        // 行動をオブジェクトに反映してキャラクターを動かすAction層。
        private BodyController _bodyController;
        // 各層で値を読み書きする用の黒板。
        private BlackBoard _blackBoard;
        // デバッグ用のステータス表示UI。
        private DebugStatusUI _debugStatusUI;

        // 非表示にする非同期処理を実行中フラグ。
        // 二重に処理を呼ばないために必要。
        private bool _isCleanupRunning;

        /// <summary>
        /// 敵の各種パラメータを参照する。実行中に変化しない値はこっち。
        /// </summary>
        public IReadonlyEnemyParams Params
        {
            get
            {
                if (_params == null) _params = GetComponent<EnemyParams>();
                return _params;
            }
        }
        /// <summary>
        /// 敵の状態を参照する。実行中に変化する値はこっち。
        /// </summary>
        public IReadonlyBlackBoard BlackBoard => _blackBoard;

        private void Awake()
        {
            Transform player = FindPlayer();
            // Animatorが1つしか無い前提。
            Animator animator = GetComponentInChildren<Animator>();
            // 雑魚敵は装備が1つ。
            Equipment equip = GetComponent<Equipment>();
            // 敵はオブジェクトの構成が統一されているので名前で取得で十分？
            Transform offset = FindOffset();
            Transform rotate = FindRotate();

            _params = GetComponent<EnemyParams>();
            _blackBoard = new BlackBoard(gameObject.name);

            // Perception
            _perception = new Perception(transform, _params, _blackBoard, player.transform);
            _fireRate = new FireRate(_params, _blackBoard, equip);
            _eyeSensor = new EyeSensor(transform, _params, _blackBoard, rotate);
            _hitPoint = new HitPoint(_params, _blackBoard);
            _overrideOrder = new OverrideOrder(_blackBoard);
            // Brain
            _utilityEvaluator = new UtilityEvaluator(_params, _blackBoard);
            _behaviorTree = new BehaviorTree(transform, _params, _blackBoard);
            // Action
            _bodyController = new BodyController(transform, _params, _blackBoard, offset, rotate, _models,
                animator, _effects, _hitBoxes);

#if UNITY_EDITOR
            _debugStatusUI = new DebugStatusUI(transform, _params, _blackBoard);
#endif
        }

        private void OnEnable()
        {
            _eyeSensor.Enable();
        }

        private void Start()
        {
            EnemyManager.Register(this);

            _perception.Init();
            _hitPoint.Init();          
        }

        private void Update()
        {
            _perception.Update();
            _eyeSensor.Update();
            _fireRate.UpdateIfAttacked();
            _hitPoint.Update();
            // 命令で上書きするのでPerception層の一番最後。
            _overrideOrder.Update();

            // 優先度順で全ての行動に対する制御を決める。
            // 後々、優先度の低い行動は省くような処理が入るかもしれない。
            IReadOnlyList<Choice> eval = _utilityEvaluator.Evaluate();
            for (int i = 0; i < eval.Count; i++)
            {
                _behaviorTree.Run(eval[i]);
            }

            // オブジェクトに諸々を反映させているので結果をハンドリングする。
            // 完了が返ってきた場合は、続けて後始末処理を呼び出す。
            // 非表示前処理 -> LateUpdate -> 次フレームのUpdate -> 非表示 の順で呼ばれる。
            if (_bodyController.Update() == BodyController.Result.Complete && !_isCleanupRunning)
            {
                _isCleanupRunning = true;
                StartCoroutine(CleanupAsync());
            }
        }

        // Updateのタイミングで視界に捉えたもの一覧を黒板に書き込み、LateUpdateでフレームを跨ぐ前に消す。
        // このタイミングで書き込んだ内容を消しているので、ギズモへの描画が難しい。
        private void LateUpdate()
        {
            _eyeSensor.ClearCaptureTargets();
            _overrideOrder.ClearOrderedTrigger();
            _behaviorTree.ClearBlackBoardWritedValues();
        }

        // 後始末、Update内から呼び出す。
        private IEnumerator CleanupAsync()
        {
            _perception.Dispose();
            _bodyController.Dispose();

            // 次フレームのUpdateの後まで待つ。
            yield return null;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _eyeSensor.Disable();
        }

        private void OnDestroy()
        {
            // 登録解除は死亡したタイミングではなく、ゲームが終了するタイミングになっている。
            // 死亡した敵かの判定が出来るようにするため。
            EnemyManager.Release(this);

            _perception.Dispose();
            _bodyController.Dispose();
        }

        private void OnDrawGizmos()
        {
            _eyeSensor?.Draw();
            _perception?.Draw();
            _debugStatusUI?.Draw();
        }

        /// <summary>
        /// 外部から敵の行動を制御する。
        /// </summary>
        public void Order(EnemyOrder order)
        {
            // Updateが呼ばれない状態ではバッファの命令を削除しないので弾く。
            if (gameObject.activeSelf) _overrideOrder.Buffer(order);
        }

        /// <summary>
        /// 攻撃させる。
        /// </summary>
        public void Attack() => _overrideOrder.Buffer(EnemyOrder.Type.Attack);

        /// <summary>
        /// ポーズさせる。
        /// </summary>
        public void Pause() => _overrideOrder.Buffer(EnemyOrder.Type.Pause);

        /// <summary>
        /// ポーズを解除させる。
        /// </summary>
        public void Resume() => _overrideOrder.Buffer(EnemyOrder.Type.Resume);

        /// <summary>
        /// ダメージ処理。
        /// </summary>
        public void Damage(int value, string weapon) => _hitPoint.Damage(value, weapon);
    }
}