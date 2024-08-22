using System.Collections;
using UnityEngine;
using System.Collections.Generic;

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
        [SerializeField] private EnemyEffects _effects;
        [SerializeField] private Collider[] _hitBoxes;

        private EnemyParams _params;
        private BlackBoard _blackBoard;
        private Perception _perception;
        private StateMachine _stateMachine;

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
                renderers: _renderers,
                effects: _effects,
                hitBoxes: _hitBoxes,
                equipment: GetComponent<Equipment>() // 雑魚敵は装備が1つ。
                );

            _params = requiredRef.EnemyParams;
            _blackBoard = requiredRef.BlackBoard;

            _perception = new Perception(requiredRef);
            _stateMachine = new StateMachine(requiredRef);
        }

        private void Start()
        {
            EnemyManager.Register(this);

            _perception.InitializeOnStart();
        }

        private void Update()
        {
            _perception.Update();

            // オブジェクトに諸々を反映させているので結果をハンドリングする。
            // 完了が返ってきた場合は、続けて後始末処理を呼び出す。
            // 非表示前処理 -> LateUpdate -> 次フレームのUpdate -> 非表示 の順で呼ばれる。
            if (_stateMachine.Update() == StateMachine.Result.Complete && !_isCleanupRunning)
            {
                _isCleanupRunning = true;
                StartCoroutine(CleanupAsync());
            }
        }

        // 後始末、Update内から呼び出す。
        private IEnumerator CleanupAsync()
        {
            _stateMachine.Dispose();

            // 次フレームのUpdateの後まで待つ。
            yield return null;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            // 死亡した敵かの判定が出来るようにするため、ゲームが終了するタイミングで登録解除。
            EnemyManager.Release(this);

            _stateMachine.Dispose();
        }

        private void OnDrawGizmosSelected()
        {
            _perception?.Draw();
        }

        /// <summary>
        /// 外部から敵の行動を制御する。
        /// </summary>
        public void Order(EnemyOrder order) => _perception.Order(order);

        /// <summary>
        /// 攻撃させる。
        /// </summary>
        public void Attack() => _perception.Order(EnemyOrder.Type.Attack);

        /// <summary>
        /// ポーズさせる。
        /// </summary>
        public void Pause() => _perception.Order(EnemyOrder.Type.Pause);

        /// <summary>
        /// ポーズを解除させる。
        /// </summary>
        public void Resume() => _perception.Order(EnemyOrder.Type.Resume);

        /// <summary>
        /// ダメージ処理。
        /// </summary>
        public void Damage(int value, string weapon) => _perception.Damage(value, weapon);
    }
}