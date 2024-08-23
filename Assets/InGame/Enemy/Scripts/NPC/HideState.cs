using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.NPC
{
    public class HideState : State<StateKey>
    {
        public HideState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Hide;

            Ref.Body.RendererEnable(false);
            Ref.Effector.ThrusterEnable(false);
            Ref.Effector.TrailEnable(false);
        }

        protected override void Exit()
        {
            Ref.Body.RendererEnable(true);
            Ref.Effector.ThrusterEnable(true);
            Ref.Effector.TrailEnable(true);
        }

        protected override void Stay()
        {
            bool isPlay = Ref.BlackBoard.IsPlay;
            if (isPlay)
            {
                TryChangeState(StateKey.Approach);
            }
        }
    }
}
