using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel.FSM
{
    public class HideState : State
    {
        public HideState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        protected RequiredRef Ref { get; private set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Hide;

            Ref.Body.RendererEnable(false);
            Ref.Body.HitBoxEnable(false);
        }

        protected override void Exit()
        {
            Ref.Body.RendererEnable(true);
            Ref.Body.HitBoxEnable(true);
        }

        protected override void Stay()
        {
            bool isExpand = Ref.BlackBoard.IsExpand;
            if (isExpand) { TryChangeState(StateKey.Expand); return; }
        }
    }
}
