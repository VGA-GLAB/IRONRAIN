﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss.FSM
{
    public class BrokenState : State
    {
        private BlackBoard _blackBoard;

        public BrokenState(BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
        }

        public override StateKey Key => StateKey.Broken;

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {

        }
    }
}