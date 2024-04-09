using System.Collections;
using UnityEngine;

namespace Enemy.Control
{
    public class EnemyController : MonoBehaviour, IDamageable
    {
        [Header("ヒエラルキー上の別オブジェクトへの参照")]
        [SerializeField] private Transform _player;
        [SerializeField] private SurroundingPool _surroundingPool;
        [Header("子やプレハブへの参照")]
        [SerializeField] private Transform _offset;
        [SerializeField] private Transform _rotate;
        [SerializeField] private Animator _animator;
        [SerializeField] private EnemyParams _enemyParams;
        [SerializeField] private Effect[] _effects;
        [Header("他人が作った機能への参照")]
        [SerializeField] private GameObject _approach; // まだ仕様が決まっていないのでとりあえずGameObjectで参照する。

        private Transform _transform;
        private Perception _perception;
        private Brain _brain;
        private Action _action;
        private BlackBoard _blackBoard;
        private DebugStatusUI _debugStatusUI;

        // 非表示にする非同期処理を実行中フラグ。
        // 二重に処理を呼ばないために必要。
        private bool _isCleanupRunning;

        private void Awake()
        {
            // まだ仕様が決まっていないので、とりあえずインターフェースを噛ませておく。
            // タイムラインやアニメーションになるかもしれない。
            IApproach approach = _approach != null ? _approach.GetComponent<IApproach>() : null;

            _transform = transform;
            _blackBoard = new BlackBoard();
            _perception = new Perception(_transform, _rotate, _player, _enemyParams, _blackBoard, _surroundingPool);
            _brain = new Brain(_transform, _rotate, _enemyParams, _blackBoard, approach);
            _action = new Action(_transform, _offset, _rotate, _animator, _blackBoard, _enemyParams, _effects, GetComponent<IWeapon>());
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
            _action.OnDisableEvent();
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