using Enemy.Control.BT;

namespace Enemy.Control
{
    public class WriteActionPlan : Node
    {
        private Choice _choice;
        private BlackBoard _blackBoard;

        public WriteActionPlan(Choice choice, BlackBoard blackBoard)
        {
            _choice = choice;
            _blackBoard = blackBoard;
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override State Stay()
        {
            _blackBoard.AddActionOptions(_choice);
            return State.Success;
        }
    }
}
