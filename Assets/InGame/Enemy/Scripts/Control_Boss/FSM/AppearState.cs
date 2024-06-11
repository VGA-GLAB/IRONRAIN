using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss.FSM
{
    /// <summary>
    /// 登場するステート。
    /// ボス戦開始後、一番最初に実行される。
    /// </summary>
    public class AppearState : State
    {
        private BlackBoard _blackBoard;

        public AppearState(BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
        }

        public override StateKey Key => StateKey.Appear;

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
        }

        public override void Dispose()
        {
        }
    }
}