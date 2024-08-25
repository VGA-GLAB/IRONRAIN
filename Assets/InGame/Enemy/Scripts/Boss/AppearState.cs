namespace Enemy.Boss
{
    /// <summary>
    /// 登場するステート。
    /// ボス戦開始後、一番最初に実行される。
    /// </summary>
    public class AppearState : State<StateKey>
    {
        public AppearState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Appear;

            // 登場後、戦闘開始と同時にレーダーマップに表示。
            AgentScript agent = Ref.AgentScript;
            if (agent != null) agent.EnemyGenerate();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            Ref.Effector.ThrusterEnable(true);
            Ref.Effector.TrailEnable(true);

            /* 登場演出ｺｺ */

            TryChangeState(StateKey.Idle);
        }

        public override void Dispose()
        {
        }
    }
}