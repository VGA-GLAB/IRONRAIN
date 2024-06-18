using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss.BT
{
    public class LookAtPlayer : Enemy.Control.BT.Node
    {
        private ActionPlan.Look _plan;
        private BlackBoard _blackBoard;

        public LookAtPlayer(BlackBoard blackBoard)
        {
            _plan = new ActionPlan.Look(Choice.Chase);
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
            // 前方向の値を変更することでプレイヤーの方向を向かせる。
            _plan.Forward = _blackBoard.TransformToPlayerDirection;
            _blackBoard.LookPlans.Enqueue(_plan);

            return State.Success;
        }
    }
}