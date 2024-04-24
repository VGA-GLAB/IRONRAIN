using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// 何もしない状態のステート
    /// 各ステート完了後に戻ってくる。
    /// </summary>
    public class IdleState : State
    {
        private BlackBoard _blackBoard;

        public IdleState(BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
        }

        public override StateKey Key => StateKey.Idle;

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            // 優先度の一番高い行動を選択して遷移する。
            if (_blackBoard.ActionOptions.TryPeek(out ActionPlan plan))
            {
                if (plan.Choice == Choice.Approach) TryChangeState(stateTable[StateKey.Approach]);
                if (plan.Choice == Choice.Chase) TryChangeState(stateTable[StateKey.Battle]);
                if (plan.Choice == Choice.Attack) TryChangeState(stateTable[StateKey.Battle]);
                if (plan.Choice == Choice.Escape) TryChangeState(stateTable[StateKey.Escape]);
                if (plan.Choice == Choice.Broken) TryChangeState(stateTable[StateKey.Broken]);
            }
        }
    }
}
