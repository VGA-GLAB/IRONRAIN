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
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationEvent _animationEvent;
        [SerializeField] private Effect[] _effects;
        [Header("他人が作った機能への参照")]
        [SerializeField] private GameObject _approach; // まだ仕様が決まっていないのでとりあえずGameObjectで参照する。

        // 注入する依存関係
        private Transform _player;
        private SlotPool _surroundingPool;

        private Transform _transform;
        private Perception _perception;
        private Brain _brain;
        private Action _action;
        private BlackBoard _blackBoard;
        private DebugStatusUI _debugStatusUI;

        // 非表示にする非同期処理を実行中フラグ。
        // 二重に処理を呼ばないために必要。
        private bool _isCleanupRunning;

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

            _transform = transform;
            _blackBoard = new BlackBoard();
            _perception = new Perception(_transform, _rotate, _player, _enemyParams, _blackBoard, _surroundingPool);
            _brain = new Brain(_transform, _rotate, _enemyParams, _blackBoard, approach);
            _action = new Action(_transform, _offset, _rotate, _animator, _animationEvent, _blackBoard, _enemyParams, _effects, GetComponent<IEquipment>());
            _debugStatusUI = new DebugStatusUI(_transform, _enemyParams, _blackBoard);
        }

        private void Start()
        {
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
    }
}