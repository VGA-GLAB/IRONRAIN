using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// 生成後、画面に映っていない状態のステート
    /// </summary>
    public class HideState : State
    {
        private BlackBoard _blackBoard;
        private Body _body;

        public HideState(BlackBoard blackBoard, Body body)
        {
            _blackBoard = blackBoard;
            _body = body;
        }

        public override StateKey Key => StateKey.Hide;

        protected override void Enter()
        {
            _body.RendererEnable(false);
        }

        protected override void Exit()
        {
            _body.RendererEnable(true);
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            if(IsExit()) TryChangeState(stateTable[StateKey.Idle]);
        }

        // 隠れる行動が選ばれていなければ遷移する。
        private bool IsExit()
        {
            foreach (ActionPlan plan in _blackBoard.ActionOptions)
            {
                if (plan.Choice == Choice.Hide)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
