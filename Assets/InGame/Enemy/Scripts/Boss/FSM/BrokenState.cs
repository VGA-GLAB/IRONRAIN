using System.Collections.Generic;

namespace Enemy.Boss.FSM
{
    public class BrokenState : State
    {
        private BlackBoard _blackBoard;

        public BrokenState(IReadOnlyDictionary<StateKey, State> states, BlackBoard blackBoard) : base(states)
        {
            _blackBoard = blackBoard;
        }

        protected override void Enter()
        {
            // 撃破されたときの音
            AudioWrapper.PlaySE("SE_Kill");
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {

        }
    }
}