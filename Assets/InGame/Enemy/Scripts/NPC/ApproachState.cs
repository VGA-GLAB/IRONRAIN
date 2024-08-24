using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.NPC
{
    public class ApproachState : State<StateKey>
    {
        public ApproachState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Approach;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            float dist = Ref.BlackBoard.TargetSqrDistance;
            float DefeatDist = Ref.NpcParams.DefeatSqrDistance;
            if (dist < DefeatDist) { TryChangeState(StateKey.Action); return; }

            float spd = Ref.NpcParams.MoveSpeed;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            Vector3 dir = Ref.BlackBoard.TargetDirection;
            Vector3 velo = dir * dt * spd;
            Ref.Body.Move(velo);
        }
    }
}
