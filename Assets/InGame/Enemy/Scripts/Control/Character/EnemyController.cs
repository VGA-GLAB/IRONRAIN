using UnityEngine;

namespace Enemy.Control
{
    public class EnemyController : MonoBehaviour
    {
        [Header("ヒエラルキー上の別オブジェクトへの参照")]
        [SerializeField] private Transform _player;
        [SerializeField] private SurroundingPool _surroundingPool;
        [Header("子やプレハブへの参照")]
        [SerializeField] private Transform _offset;
        [SerializeField] private Transform _rotate;
        [SerializeField] private Animator _animator;
        [SerializeField] private EnemyParams _enemyParams;

        private Transform _transform;
        private Perception _perception;
        private Brain _brain;
        private Action _action;
        private BlackBoard _blackBoard;

        private void Awake()
        {
            _transform = transform;
            _blackBoard = new BlackBoard();
            _perception = new Perception(_transform, _rotate, _player, _enemyParams, _blackBoard, _surroundingPool);
            _brain = new Brain(_transform, _enemyParams, _blackBoard);
            _action = new Action(_transform, _offset, _rotate, _animator, _blackBoard);
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

        private void Update()
        {
            _perception.UpdateEvent();
            _brain.UpdateEvent();
            _action.UpdateEvent();
        }

        private void LateUpdate()
        {
            _perception.LateUpdateEvent();
        }

        private void OnDrawGizmos()
        {
            _perception?.OnDrawGizmosEvent();
        }
    }
}