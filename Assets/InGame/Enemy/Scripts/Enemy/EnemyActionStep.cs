using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyActionStep : BattleActionStep
    {
        public EnemyActionStep(RequiredRef requiredRef, params EnemyActionStep[] next) : base(next)
        {
            Ref = requiredRef;
        }

        public RequiredRef Ref { get; }

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            return this;
        }
    }
}
