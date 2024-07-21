using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Control.FSM;

namespace Enemy.Control.Boss.FSM
{
    /// <summary>
    /// 左腕破壊~2回目のQTEまでの一連のイベントのステート。
    /// </summary>
    public class QteEventState : State
    {
        public enum Step
        {
            Other,
            BreakLeftArm,
            FirstQte,
            SecondQte,
        }

        private BlackBoard _blackBoard;
        private AgentScript _agentScript;

        public QteEventState(BlackBoard blackBoard, AgentScript agentScript)
        {
            _blackBoard = blackBoard;
            _agentScript = agentScript;
        }

        public override StateKey Key => StateKey.QteEvent;

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
            if (_agentScript != null) _agentScript.EnemyDestory();
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            while(_blackBoard.ActionPlans.TryDequeue(out ActionPlan plan))
            {
                if (plan.Choice == Choice.BreakLeftArm)
                {
                    Debug.Log("敵側でプレイヤーの左腕破壊演出の処理が呼ばれている状態");
                }
                else if (plan.Choice == Choice.FirstQte)
                {
                    Debug.Log("敵側で1回目のQTEの処理が呼ばれている状態");
                }
                else if (plan.Choice == Choice.SecondQte)
                {
                    Debug.Log("敵側で2回目のQTEの処理が呼ばれている状態");
                }
                else if (plan.Choice == Choice.Broken)
                {
                    TryChangeState(stateTable[StateKey.Idle]);
                    return;
                }
            }
        }
    }
}