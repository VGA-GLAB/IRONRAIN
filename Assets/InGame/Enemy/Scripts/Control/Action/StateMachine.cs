using Enemy.Control.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 行動を反映させる用途のステートマシン。
    /// </summary>
    public class StateMachine
    {
        private Dictionary<StateKey, State> _stateTable;
        private State _currentState;

        public StateMachine(BlackBoard blackBoard, Body body, BodyAnimation animation, Effector effector, 
            IEquipment equipment)
        {
            _stateTable = new Dictionary<StateKey, State>
            {
                { StateKey.Approach, new ApproachState(blackBoard, body, animation) },
                { StateKey.Battle, new BattleState(blackBoard, body, animation, equipment) },
                { StateKey.Broken, new BrokenState(animation, effector, blackBoard) },
                { StateKey.Escape, new EscapeState(blackBoard, body, animation) },
                { StateKey.Idle, new IdleState(blackBoard) },
                { StateKey.Hide, new HideState(blackBoard, body) },
            };

            _currentState = _stateTable[StateKey.Idle];
        }

        /// <summary>
        /// ステートを1フレームぶん更新
        /// </summary>
        public void Update()
        {
            _currentState = _currentState.Update(_stateTable);
        }

        /// <summary>
        /// ステートの破棄
        /// </summary>
        public void Destroy()
        {
            foreach(KeyValuePair<StateKey, State> s in _stateTable)
            {
                s.Value.Destroy();
            }
        }
    }
}
