namespace Enemy.Boss.FSM
{
    public class HideState : State
    {
        public HideState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Hide;

            Ref.Body.RendererEnable(false);
        }

        protected override void Exit()
        {
            Ref.Body.RendererEnable(true);
        }

        protected override void Stay()
        {
            // ボス戦が始まった場合は登場状態に遷移。
            bool isBossStarted = Ref.BlackBoard.IsBossStarted;
            if (isBossStarted) TryChangeState(StateKey.Appear);
        }
    }
}
