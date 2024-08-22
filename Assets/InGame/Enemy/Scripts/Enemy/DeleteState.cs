namespace Enemy
{
    /// <summary>
    /// 画面から削除され、もう使わない状態のステート。
    /// </summary>
    public class DeleteState : State<StateKey>
    {
        public DeleteState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Delete;
            Ref.BlackBoard.IsCleanupReady = true;

            Ref.Body.RendererEnable(false);
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {

        }
    }
}
