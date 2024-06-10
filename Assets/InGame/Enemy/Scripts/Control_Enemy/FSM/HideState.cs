using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// 生成後、画面に表示されていない状態のステート。
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
            // 隠れる行動が選ばれているかチェック。
            bool isExit = true;
            foreach (ActionPlan plan in _blackBoard.ActionPlans)
            {
                if (plan.Choice == Choice.Hide) isExit = false;
            }

            // 選ばれていない場合はアイドルに遷移。
            if(isExit) TryChangeState(stateTable[StateKey.Idle]);
        }
    }
}
