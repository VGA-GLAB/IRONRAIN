using System.Collections;
using UnityEngine;
using VContainer;

namespace Enemy.Control
{
    public class EnemyController : MonoBehaviour, IDamageable
    {
        [Header("----------プランナーが弄る値----------")]
        [SerializeField] private EnemyParams _enemyParams;
        [Header("------------------------------------")]
        [Header("子やプレハブへの参照")]
        [SerializeField] private Transform _offset;
        [SerializeField] private Transform _rotate;
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationEvent _animationEvent;
        [SerializeField] private Effect[] _effects;
        [Header("他人が作った機能への参照")]
        [SerializeField] private GameObject _approach; // まだ仕様が決まっていないのでとりあえずGameObjectで参照する。

        // 注入する依存関係
        private Transform _player;
        private SlotPool _surroundingPool;

        private Perception _perception;
        private Brain _brain;
        private Action _action;
        private BlackBoard _blackBoard;
        private DebugStatusUI _debugStatusUI;

        // 非表示にする非同期処理を実行中フラグ。
        // 二重に処理を呼ばないために必要。
        private bool _isCleanupRunning;

        /// <summary>
        /// 敵の種類を判定する値を返す。実行中に変化しない値はこっち。
        /// </summary>
        public IReadonlyEnemyParams Params => _enemyParams;
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
            // 識別用にランダムな名前を付ける。
            gameObject.RandomName();

            // 依存関係をチェック
            if (_player == null || _surroundingPool == null)
            {
                Debug.LogWarning($"依存関係の構築に失敗: Player:{_player}, SurroundingPool:{_surroundingPool}");
            }

            // まだ仕様が決まっていないので、とりあえずインターフェースを噛ませておく。
            // タイムラインやアニメーションになるかもしれない。
            IApproach approach = _approach != null ? _approach.GetComponent<IApproach>() : null;

            _blackBoard = new BlackBoard();
            _perception = new Perception(transform, _rotate, _player, _enemyParams, _blackBoard, _surroundingPool);
            _brain = new Brain(transform, _rotate, _enemyParams, _blackBoard, approach);
            _action = new Action(transform, _offset, _rotate, _animator, _renderers, 
                _animationEvent, _blackBoard, _enemyParams, _effects, GetComponent<IEquipment>());
            _debugStatusUI = new DebugStatusUI(transform, _enemyParams, _blackBoard);
        }

        private void Start()
        {
            EnemyManager.Register(this);
            _perception.OnStartEvent();
        }

        private void OnEnable()
        {
            _perception.OnEnableEvent();
        }

        private void OnDisable()
        {
            _perception.OnDisableEvent();
        }

        private void OnDestroy()
        {
            EnemyManager.Release(this);
            _action.OnDestroyEvent();
        }

        private void Update()
        {
            _perception.UpdateEvent();
            _brain.UpdateEvent();

            // Action内でオブジェクトに諸々を反映させているので結果をハンドリングする。
            // 完了が返ってきた場合は、続けて後始末処理を呼び出す。
            // OnPreCleanup -> LateUpdate -> 次フレームのUpdate -> 非表示 の順で呼ばれる。
            if (_action.UpdateEvent() == LifeCycle.Result.Complete && !_isCleanupRunning)
            {
                _isCleanupRunning = true;
                StartCoroutine(CleanupAsync());
            }
        }

        private void LateUpdate()
        {
            _perception.LateUpdateEvent();
            _brain.LateUpdateEvent();
        }

        // 後始末、Update内から呼び出す。
        private IEnumerator CleanupAsync()
        {
            _brain.OnPreCleanup();
            _action.OnPreCleanup();
         
            // 次フレームのUpdateの後まで待つ。
            yield return null;
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            _perception?.OnDrawGizmosEvent();
            _debugStatusUI?.Draw();
        }

        void IDamageable.Damage(int value, string weapon)
        {
            _perception.OnDamaged(value, weapon);
        }

        /// <summary>
        /// 任意のタイミングで攻撃する。
        /// </summary>
        public void Attack()
        {
            _perception.OnAttackEvent();
        }

        /// <summary>
        /// 任意のタイミングでポーズする。
        /// </summary>
        public void Pause()
        {
            _perception.OnPauseEvent();
        }

        /// <summary>
        /// 任意のタイミングでポーズ解除する。
        /// </summary>
        public void Resume()
        {
            _perception.OnResumeEvent();
        }
    }
}