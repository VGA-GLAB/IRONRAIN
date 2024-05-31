using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Enemy.Control.Boss
{
    public class BossController : MonoBehaviour
    {
        [Header("----------プランナーが弄る値----------")]
        [SerializeField] private BossParams _bossParams;
        [Header("------------------------------------")]

        // 注入する依存関係
        private BossStage _stage;
        private Transform _player;

        private Transform _transform;
        private List<FunnelController> _funnels;
        private BlackBoard _blackBoard;
        private Perception _perception;
        private Brain _brain;
        private Action _action;

        [Inject]
        private void Construct(BossStage stage, Transform player)
        {
            _stage = stage;
            _player = player;
        }

        private void Awake()
        {
            _transform = transform;
            _funnels = new List<FunnelController>();
            _blackBoard = new BlackBoard();
            _perception = new Perception(_transform, _player, _blackBoard, _stage);
            _brain = new Brain(_blackBoard);
            _action = new Action(_blackBoard, _transform, _funnels);
        }

        private void Start()
        {
            // ファンネルと相互に参照させる。
            FunnelController.RegisterOwner(this, _funnels);

            _perception.OnStartEvent();
            _action.OnStartEvent();
        }

        private void Update()
        {
            _perception.UpdateEvent();
            _brain.UpdateEvent();
            _action.UpdateEvent();
        }

        private void LateUpdate()
        {
            _brain.LateUpdateEvent();
        }

        private void OnDisable()
        {
            _perception.OnDisableEvent();
        }

        private void OnDrawGizmos()
        {
            _perception?.OnDrawGizmosEvent();
        }
    }
}
