using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Funnel;
using IronRain.Player;

namespace Enemy.Boss
{
    // 一時的に画面から消して動作を止めたい場合: xxx
    // これ以上動かさず、削除する場合: xxx

    /// <summary>
    /// ボスを操作する。
    /// </summary>
    [RequireComponent(typeof(BossParams))]
    public class BossController : Character, IDamageable
    {
        [SerializeField] private BossEffects _effects;
        [SerializeField] private Collider[] _hitBoxes;

        private BossParams _params;
        private BlackBoard _blackBoard;
        private List<FunnelController> _funnels;
        private Perception _perception;
        private StateMachine _stateMachine;

        // 非表示にする非同期処理を実行中フラグ。
        // 二重に処理を呼ばないために必要。
        private bool _isCleanupRunning;

        /// <summary>
        /// ボスの状態を参照する。実行中に変化しない値はこっち。
        /// </summary>
        public IReadonlyBossParams Params => _params;

        /// <summary>
        /// ボスの状態を参照する。実行中に変化する値はこっち。
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
                pointP: FindAnyObjectByType<PointP>().transform,
                bossParams: GetComponent<BossParams>(),
                blackBoard: new BlackBoard(gameObject.name),
                animator: GetComponentInChildren<Animator>(),
                renderers: _renderers,
                effects: _effects,
                hitBoxes: _hitBoxes,
                meleeEquip: GetComponent<MeleeEquipment>(),
                rangeEquip: GetComponent<RangeEquipment>(),
                funnels: new List<FunnelController>()
                );

            _params = requiredRef.BossParams;
            _blackBoard = requiredRef.BlackBoard;
            _funnels = requiredRef.Funnels;

            _perception = new Perception(requiredRef);
            _stateMachine = new StateMachine(requiredRef);
        }

        private void Start()
        {
            EnemyManager.Register(this);
            FunnelController.RegisterOwner(this, _funnels);

            _perception.InitializeOnStart();
        }

        private void Update()
        {
            _perception.Update();
            
            // オブジェクトに諸々を反映させているので結果をハンドリングする。
            // 完了が返ってきた場合は、続けて後始末処理を呼び出す。
            // 非表示前処理 -> LateUpdate -> 次フレームのUpdate -> 非表示 の順で呼ばれる。
            if (_stateMachine.Update() == Result.Complete && !_isCleanupRunning)
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
            // 登録解除は死亡したタイミングではなく、ゲームが終了するタイミングになっている。
            // 死亡した敵かの判定が出来るようにするため。
            EnemyManager.Release(this);

            _stateMachine.Dispose();
        }

        private void OnDrawGizmosSelected()
        {
            _perception?.Draw();
        }

        /// <summary>
        /// 外部から敵の行動を制御する。
        /// /// </summary>
        public void Order(EnemyOrder order) => _perception.Order(order);

        /// <summary>
        /// ダメージ処理。
        /// </summary>
        public void Damage(int value, string weapon = "") => _perception.Damage(value, weapon);
    }
}
