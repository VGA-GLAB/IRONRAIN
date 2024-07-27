using System.Collections;
using System.Collections.Generic;
using Enemy.Control.FSM;
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
        private Effector _effector;

        public AppearState(BlackBoard blackBoard, Effector effector)
        {
            _blackBoard = blackBoard;
            _effector = effector;
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
            /* 登場演出ｺｺ */
            _effector.ThrusterEnable(true);
            _effector.TrailEnable(true);
            
            TryChangeState(stateTable[StateKey.Idle]);
        }

        public override void Dispose()
        {
        }
    }
}