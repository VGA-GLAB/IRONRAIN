using UnityEngine;

namespace Enemy.Control.BT
{
    /// <summary>
    /// 逃げる際に垂直移動する。
    /// </summary>
    public class MoveVertical : Node
    {
        private ActionPlan.Move _plan;
        private EnemyParams _params;
        private BlackBoard _blackBoard;

        public MoveVertical(EnemyParams enemyParams, BlackBoard blackBoard)
        {
            _plan = new ActionPlan.Move(Choice.Escape);
            _params = enemyParams;
            _blackBoard = blackBoard;
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override State Stay()
        {
            // 上に逃げる。
            _plan.Direction = Vector3.up;
            _plan.Speed = _params.Battle.EscapeSpeed;
            _blackBoard.MovePlans.Enqueue(_plan);

            return State.Success;
        }
    }
}
