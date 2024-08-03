using UnityEngine;

namespace Enemy.Control.BT
{
    /// <summary>
    /// プレイヤーを向く。
    /// </summary>
    public class LookAtPlayer : Node
    {
        private ActionPlan.Look _plan;
        private BlackBoard _blackBoard;

        public LookAtPlayer(Choice choice, BlackBoard blackBoard)
        {
            _plan = new ActionPlan.Look(choice);
            _blackBoard = blackBoard;
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
            Vector3 dir = _blackBoard.TransformToPlayerDirection;
            dir.y = 0;

            // 前方向の値を変更することでプレイヤーの方向を向かせる。
            _plan.Forward = dir;
            _blackBoard.LookPlans.Enqueue(_plan);

            return State.Success;
        }
    }
}
