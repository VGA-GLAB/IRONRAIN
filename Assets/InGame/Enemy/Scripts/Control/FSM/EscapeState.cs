using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// 画面外に逃げ出すステート
    /// </summary>
    public class EscapeState : State
    {
        private MoveApply _moveApply;

        public EscapeState(BlackBoard blackBoard, Body body, BodyAnimation animation)
        {
            _moveApply = new MoveApply(Choice.Escape, blackBoard, body, animation);
        }

        public override StateKey Key => StateKey.Escape;

        protected override void Enter()
        {

        }

        protected override void Exit()
        {

        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            _moveApply.Run();
        }
    }
}
