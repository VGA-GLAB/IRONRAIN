using System.Collections;
using UnityEngine;

namespace Enemy.NPC
{
    /// <summary>
    /// 追跡シーン、敵との乱戦中に登場するNPC。
    /// 目標に向かって移動し、一定距離まで近づいたら撃破、そのまま直進して退場する。
    /// </summary>
    [RequireComponent(typeof(BuddyNpcParams))]
    public class BuddyNpcController : Character
    {
        [SerializeField] private NpcEffects _effects;

        private BuddyNpcParams _params;
        private Perception _perception;
        private StateMachine _stateMachine;

        // 非表示にする非同期処理を実行中フラグ。
        // 二重に処理を呼ばないために必要。
        private bool _isCleanupRunning;

        /// <summary>
        /// 各種パラメータを参照する。実行中に変化しない値はこっち。
        /// </summary>
        public BuddyNpcParams Params
        {
            get
            {
                if (_params == null) _params = GetComponent<BuddyNpcParams>();
                return _params;
            }
        }

        private void Awake()
        {
            // 必要な参照をまとめる。
            RequiredRef requiredRef = new RequiredRef(
                transform: transform,
                player: FindPlayer(),
                offset: FindOffset(),
                rotate: FindRotate(),
                npcParams: GetComponent<BuddyNpcParams>(),
                blackBoard: new BlackBoard(gameObject.name),
                animator: GetComponentInChildren<Animator>(),
                renderers: _renderers,
                effects: _effects
                );

            _params = requiredRef.NpcParams;

            _perception = new Perception(requiredRef);
            _stateMachine = new StateMachine(requiredRef);
        }

        private void Start()
        {
            EnemyManager.Register(this);

            _perception.InitializeOnStart();
        }

        // Updateだとプレイヤーとの更新間隔がズレて移動がガクガクするので、NPCだけFixedUpdateで更新。
        private void FixedUpdate()
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
            // イベント終了後も参照されるかもしれないので、登録解除するタイミングはゲーム終了時。
            EnemyManager.Release(this);
        }

        private void OnDrawGizmosSelected()
        {
            _params?.Draw();
        }

        /// <summary>
        /// 登場~対象を撃破後、退場。
        /// </summary>
        public void Play() => _perception.Play();
    }
}
