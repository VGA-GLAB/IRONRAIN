using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.BT
{
    public class HideIfOffScreen : Node
    {
        private ActionPlan _plan;
        private BlackBoard _blackBoard;

        public HideIfOffScreen(BlackBoard blackBoard)
        {
            _plan = new ActionPlan(Choice.Hide);
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
            // とりあえずプレイヤーとの距離で判定。
            if (_blackBoard.TransformToPlayerDistance > 20.0f) // 値は適当。
            {
                _blackBoard.ActionPlans.Enqueue(_plan);
            }

            return State.Success;
        }
    }
}
