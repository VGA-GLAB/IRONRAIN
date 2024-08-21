using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel.FSM
{
    public class ExpandState : State
    {
        public ExpandState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        protected RequiredRef Ref { get; private set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Expand;

            AgentScript agent = Ref.AgentScript;
            if (agent != null) agent.EnemyGenerate();

            Ref.Effector.TrailEnable(true);
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            Vector3 offset = (Vector3)Ref.BlackBoard.ExpandOffset;
            Vector3 dx = Ref.Body.Right * offset.x;
            Vector3 dy = Ref.Body.Up * offset.y;
            Vector3 dz = Ref.Body.Forward * offset.z;
            Vector3 bp = Ref.Boss.transform.position;
            Vector3 p = bp + dx + dy + dz;
            Vector3 dir = p - Ref.Body.Position;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            float spd = Ref.FunnelParams.MoveSpeed;
            Vector3 velo = dir.normalized * dt * spd;
            if (velo.sqrMagnitude <= dir.sqrMagnitude) Ref.Body.Move(velo);
            else
            {
                Ref.Body.Warp(p);
                TryChangeState(StateKey.Battle);
            }
        }
    }
}
