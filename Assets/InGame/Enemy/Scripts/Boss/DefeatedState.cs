using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss
{
    public class DefeatedState : State<StateKey>
    {
        public DefeatedState(RequiredRef requiredRef) : base(requiredRef.States)
        {
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
        }
    }
}
