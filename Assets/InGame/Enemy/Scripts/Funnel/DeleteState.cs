﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel
{
    public class DeleteState : State<StateKey>
    {
        public DeleteState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        protected RequiredRef Ref { get; private set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Delete;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
        }
    }
}