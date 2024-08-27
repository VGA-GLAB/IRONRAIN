using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss
{
    public class BossActionStep : BattleActionStep
    {
        public BossActionStep(RequiredRef requiredRef, params BossActionStep[] next) : base(next)
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
