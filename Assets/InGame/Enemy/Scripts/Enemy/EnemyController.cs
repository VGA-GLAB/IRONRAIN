using System.Collections;
using UnityEngine;
using Enemy.DebugUse;

namespace Enemy
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
        private BlackBoard _blackBoard;
        // Perception層
        private Perception _perception;
        private FireRate _fireRate;
        private EyeSensor _eyeSensor;
        private HitPoint _hitPoint;
        private OverrideOrder _overrideOrder;
        // Action層
        private BodyController _bodyController;
        // デバッグ用なので本番環境では不要。
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
            // 必要な参照をまとめる。
            RequiredRef requiredRef = new RequiredRef(
                transform: transform,
                player: FindPlayer(),
                offset: FindOffset(),
                rotate: FindRotate(),
                enemyParams: GetComponent<EnemyParams>(),
                blackBoard: new BlackBoard(gameObject.name),
                animator: GetComponentInChildren<Animator>(), // Animatorが1つしか無い前提。
                models: _models,
                effects: _effects,
                hitBoxes: _hitBoxes,
                equipment: GetComponent<Equipment>() // 雑魚敵は装備が1つ。
                );

            _params = requiredRef.EnemyParams;
            _blackBoard = requiredRef.BlackBoard;

            _perception = new Perception(requiredRef);
            _fireRate = new FireRate(requiredRef);
            _eyeSensor = new EyeSensor(requiredRef);
            _hitPoint = new HitPoint(requiredRef);
            _overrideOrder = new OverrideOrder(requiredRef);
            _bodyController = new BodyController(requiredRef);
            _debugStatusUI = new DebugStatusUI(requiredRef);
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

            // オブジェクトに諸々を反映させているので結果をハンドリングする。
            // 完了が返ってきた場合は、続けて後始末処理を呼び出す。
            // 非表示前処理 -> LateUpdate -> 次フレームのUpdate -> 非表示 の順で呼ばれる。
            if (_bodyController.Update() == BodyController.Result.Complete && !_isCleanupRunning)
            {
                _isCleanupRunning = true;
                StartCoroutine(CleanupAsync());
            }
        }

        private void LateUpdate()
        {
            _eyeSensor.ClearCaptureTargets();
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

        private void OnDestroy()
        {
            // 死亡した敵かの判定が出来るようにするため、ゲームが終了するタイミングで登録解除。
            EnemyManager.Release(this);

            _perception.Dispose();
            _bodyController.Dispose();
        }

        private void OnDrawGizmosSelected()
        {
            _eyeSensor?.Draw();
            _perception?.Draw();
            _debugStatusUI?.Draw();
        }

        /// <summary>
        /// 外部から敵の行動を制御する。
        /// </summary>
        public void Order(EnemyOrder order) => _overrideOrder.Buffer(order);

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