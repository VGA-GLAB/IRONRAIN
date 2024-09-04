using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.NPC
{
    public class ActionState : State<StateKey>
    {
        public ActionState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Action;

            Ref.Callback.InvokeAttackAction();

            EnemyController target = Ref.NpcParams.Target;
            if (target != null) target.Damage(int.MaxValue / 2, "NPC");
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            TryChangeState(StateKey.Escape);
        }
    }
}
