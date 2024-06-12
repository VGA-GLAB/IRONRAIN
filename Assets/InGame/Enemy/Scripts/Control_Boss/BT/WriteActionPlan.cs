namespace Enemy.Control.Boss.BT
{
    public class WriteActionPlan : Enemy.Control.BT.Node
    {
        private ActionPlan _plan;
        private BlackBoard _blackBoard;

        public WriteActionPlan(Choice choice, BlackBoard blackBoard)
        {
            _plan = new ActionPlan(choice);
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
            _blackBoard.ActionPlans.Enqueue(_plan);

            return State.Success;
        }
    }
}
