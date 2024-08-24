using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.NPC
{
    public class EscapeState : State<StateKey>
    {
        public EscapeState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Escape;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            bool isOver = Ref.BlackBoard.IsLifeTimeOver;
            if (isOver) TryChangeState(StateKey.Delete);

            float spd = Ref.NpcParams.MoveSpeed;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            Vector3 dir = Ref.Body.Forward;
            Vector3 velo = dir * dt * spd;
            Ref.Body.Move(velo);
        }
    }
}
