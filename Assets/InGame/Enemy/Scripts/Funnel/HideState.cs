using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel
{
    public class HideState : State<StateKey>
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
            Ref.Effector.TrailEnable(false);
        }

        protected override void Exit()
        {
            Ref.Body.RendererEnable(true);
            Ref.Body.HitBoxEnable(true);
            Ref.Effector.TrailEnable(true);
        }

        protected override void Stay()
        {
            if (Ref.BlackBoard.Expand.IsWaitingExecute()) 
            {
                Ref.BlackBoard.Expand.Execute();

                TryChangeState(StateKey.Expand); 
                return; 
            }
        }
    }
}
