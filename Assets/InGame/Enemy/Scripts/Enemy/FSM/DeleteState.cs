namespace Enemy.FSM
{
    /// <summary>
    /// 画面から削除され、もう使わない状態のステート。
    /// </summary>
    public class DeleteState : State
    {
        private BlackBoard _blackBoard;        
        private Body _body;

        public DeleteState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            _blackBoard = requiredRef.BlackBoard;
            _body = requiredRef.Body;
        }

        protected override void Enter()
        {
            _blackBoard.CurrentState = StateKey.Delete;
            _blackBoard.IsCleanupReady = true;

            _body.RendererEnable(false);
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {

        }
    }
}
