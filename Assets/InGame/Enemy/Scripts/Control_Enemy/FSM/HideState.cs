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
            _body.ModelEnable(false);
        }

        protected override void Exit()
        {
            // 生存中ならば画面に表示。
            // 隠れている状態のまま命令によって死亡した場合、1フレームだけ画面に表示されてしまうことを防ぐ。
            if (_blackBoard.IsAlive) _body.ModelEnable(true);
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
