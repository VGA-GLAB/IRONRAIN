namespace Enemy.Boss.FSM
{
    /// <summary>
    /// 登場するステート。
    /// ボス戦開始後、一番最初に実行される。
    /// </summary>
    public class AppearState : State
    {
        private BlackBoard _blackBoard;
        private Effector _effector;

        public AppearState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            _blackBoard = requiredRef.BlackBoard;
            _effector = requiredRef.Effector;
        }

        protected override void Enter()
        {
            _blackBoard.CurrentState = StateKey.Appear;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            _effector.ThrusterEnable(true);
            _effector.TrailEnable(true);

            /* 登場演出ｺｺ */

            TryChangeState(StateKey.Idle);
        }

        public override void Dispose()
        {
        }
    }
}