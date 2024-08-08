namespace Enemy.Boss.FSM
{
    /// <summary>
    /// 登場するステート。
    /// ボス戦開始後、一番最初に実行される。
    /// </summary>
    public class AppearState : State
    {
        private Effector _effector;

        public AppearState(StateRequiredRef requiredRef) : base(requiredRef.States)
        {
            _effector = requiredRef.Effector;
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            _effector.ThrusterEnable(true);
            _effector.TrailEnable(true);

            /* 登場演出ｺｺ */

            TryChangeState(StateKey.Battle);
        }

        public override void Dispose()
        {
        }
    }
}