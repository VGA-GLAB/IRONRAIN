using UnityEngine;

namespace Enemy.Control.BT
{
    /// <summary>
    /// スロットの位置に向けて移動する。
    /// </summary>
    public class MoveToPlayer : Node
    {
        private Transform _transform;
        private BlackBoard _blackBoard;
        private EnemyParams _params;

        public MoveToPlayer(Transform transform, BlackBoard blackBoard, EnemyParams enemyParams)
        {
            _transform = transform;
            _blackBoard = blackBoard;
            _params = enemyParams;
        }

        protected override void OnBreak()
        {
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override State Stay()
        {
            // 必要な参照が無い場合は、その場に留まる。
            if (_blackBoard.Area == null)
            {
                _blackBoard.AddMovementOption(Choice.Chase, Vector3.zero, 0);
                return State.Running;
            }

            // エリアの中心位置へワープ
            _blackBoard.AddWarpOption(Choice.Chase, _blackBoard.Area.Point);
            return State.Success;
        }
    }
}