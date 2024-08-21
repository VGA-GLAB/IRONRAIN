using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel.FSM
{
    public class ReturnState : State
    {
        public ReturnState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        protected RequiredRef Ref { get; private set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Return;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            Vector3 dir = Ref.BlackBoard.BossDirection;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            float spd = Ref.FunnelParams.MoveSpeed;
            Vector3 velo = dir.normalized * dt * spd;
            if (velo.sqrMagnitude <= dir.sqrMagnitude) Ref.Body.Move(velo);
            else
            {
                TryChangeState(StateKey.Hide);
            }
        }
    }
}
