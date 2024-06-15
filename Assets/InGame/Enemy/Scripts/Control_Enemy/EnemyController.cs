using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Enemy.Control
{
    /// <summary>
    /// 敵を操作する。
    /// 一時的に画面から消して動作を止めたい場合: xxx
    /// これ以上動かさず、削除する場合: xxx
    /// </summary>
    public class EnemyController : MonoBehaviour, IDamageable
    {
        [Header("プランナーが弄る値")]
        [SerializeField] private EnemyParams _params;
        [Header("自身やプレハブへの参照")]
        [SerializeField] private Transform _offset;
        [SerializeField] private Transform _rotate;
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private Animator _animator;
        [SerializeField] private Effect[] _effects;
        [SerializeField] private Collider _damageHitBox;

        // 注入する依存関係。
        private Transform _player;
        private SlotPool _surroundingPool;
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
        public IReadonlyEnemyParams Params => _params;
        /// <summary>
        /// 敵の状態を参照する。実行中に変化する値はこっち。
        /// </summary>
        public IReadonlyBlackBoard BlackBoard => _blackBoard;

        [Inject]
        private void Construct(Transform player, SlotPool pool)
        {
            _player = player;
            _surroundingPool = pool;
        }

        private void Awake()
        {
            // 依存関係をチェック
            if (_player == null || _surroundingPool == null)
            {
                Debug.LogWarning($"依存関係の構築に失敗: Player:{_player}, SurroundingPool:{_surroundingPool}");
            }

            _blackBoard = new BlackBoard(gameObject.name);

            // Perception
            _perception = new Perception(transform, _params, _blackBoard, _player, _surroundingPool);
            _fireRate = new FireRate(_params, _blackBoard);
            _eyeSensor = new EyeSensor(transform, _params, _blackBoard, _rotate);
            _hitPoint = new HitPoint(_params, _blackBoard);
            _overrideOrder = new OverrideOrder(_blackBoard);
            // Brain
            _utilityEvaluator = new UtilityEvaluator(_params, _blackBoard);
            _behaviorTree = new BehaviorTree(transform, _params, _blackBoard);
            // Action
            _bodyController = new BodyController(transform, _params, _blackBoard, _offset, _rotate, _renderers, 
                _animator, _effects, _damageHitBox);

#if UNITY_EDITOR
            _debugStatusUI = new DebugStatusUI(transform, _params, _blackBoard);
#endif
        }

        private void OnEnable()
        {
            // Perception
            _eyeSensor.Enable();
        }

        private void Start()
        {
            EnemyManager.Register(this);

            // レーダーに表示する。
            if (TryGetComponent(out AgentScript a)) a.EnemyGenerate();

            // Perception
            _perception.Init();
            _hitPoint.Init();          
        }

        private void Update()
        {
            // Perception
            _perception.Update();
            _eyeSensor.Update();
            _fireRate.UpdateIfAttacked();
            _hitPoint.Update();
            // 命令で上書きするのでPerception層の一番最後。
            _overrideOrder.Update();     

            // Brain
            // 優先度順で全ての行動に対する制御を決める。
            // 後々、優先度の低い行動は省くような処理が入るかもしれない。
            IReadOnlyList<Choice> eval = _utilityEvaluator.Evaluate();
            for (int i = 0; i < eval.Count; i++)
            {
                _behaviorTree.Run(eval[i]);
            }

            // Action
            // オブジェクトに諸々を反映させているので結果をハンドリングする。
            // 完了が返ってきた場合は、続けて後始末処理を呼び出す。
            // OnPreCleanup -> LateUpdate -> 次フレームのUpdate -> 非表示 の順で呼ばれる。
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
            // Perception
            _eyeSensor.ClearCaptureTargets();
            // 同フレームの間だけtrueになる所謂トリガーを扱うのでフレームの最後で戻しておく。
            _overrideOrder.ClearOrderedTrigger();

            // Brain
            _behaviorTree.ClearBlackBoardWritedValues();
        }

        // 後始末、Update内から呼び出す。
        private IEnumerator CleanupAsync()
        {
            // レーダーから消す。
            if (TryGetComponent(out AgentScript a)) a.EnemyDestory();

            // Perception
            _perception.Dispose();

            // Action
            _bodyController.Dispose();

            // 次フレームのUpdateの後まで待つ。
            yield return null;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            // Perception
            _eyeSensor.Disable();
        }

        private void OnDestroy()
        {
            // 登録解除は死亡したタイミングではなく、ゲームが終了するタイミングになっている。
            // 死亡した敵かの判定が出来るようにするため。
            EnemyManager.Release(this);

            // Perception
            _perception.Dispose();

            // Action
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
        /// /// </summary>
        public void Order(EnemyOrder order)
        {
            _overrideOrder.Buffer(order);
        }

        /// <summary>
        /// 攻撃させる。
        /// </summary>
        public void Attack()
        {
            _overrideOrder.Buffer(EnemyOrder.Type.Attack);
        }

        /// <summary>
        /// ポーズさせる。
        /// </summary>
        public void Pause()
        {
            _overrideOrder.Buffer(EnemyOrder.Type.Pause);
        }

        /// <summary>
        /// ポーズを解除させる。
        /// </summary>
        public void Resume()
        {
            _overrideOrder.Buffer(EnemyOrder.Type.Resume);
        }

        /// <summary>
        /// ダメージ処理。
        /// </summary>
        public void Damage(int value, string weapon)
        {
            _hitPoint?.Damage(value, weapon);
        }
    }
}