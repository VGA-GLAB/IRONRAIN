using UnityEngine;

namespace Enemy.Boss
{
    /// <summary>
    /// 登場するステート。
    /// ボス戦開始後、一番最初に実行される。
    /// </summary>
    public class AppearState : State<StateKey>
    {
        // 移動完了後、プレイヤーの方を向く。
        private BossActionStep[] _steps;
        private BattleActionStep _currentStep;

        public AppearState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;

            _steps = new BossActionStep[4];
            _steps[3] = new ApproachEndStep(requiredRef, null);
            _steps[2] = new LookAtPlayerStep(requiredRef, _steps[3]);
            _steps[1] = new MoveToInitialLaneStep(requiredRef, _steps[2]);
            _steps[0] = new WaitPlayerReadyStep(requiredRef, _steps[1]);

            _currentStep = _steps[0];
        }

        private RequiredRef Ref { get; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Appear;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            _currentStep = _currentStep.Update();

            if (_currentStep.ID == nameof(ApproachEndStep)) TryChangeState(StateKey.Idle);
        }

        public override void Dispose()
        {
        }
    }

    /// <summary>
    /// プレイヤーの準備が完了し、フィールドの参照が確保できるまで待つ。
    /// </summary>
    public class WaitPlayerReadyStep : BossActionStep
    {
        public WaitPlayerReadyStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            // 登場後、戦闘開始と同時にレーダーマップに表示。
            AgentScript agent = Ref.AgentScript;
            if (agent != null) agent.EnemyGenerate();

            Ref.Effector.ThrusterEnable(true);
            Ref.Effector.TrailEnable(true);
        }

        protected override BattleActionStep Stay()
        {
            if (Ref.Field.IsEnabled()) return Next[0];
            else return this;
        }
    }

    /// <summary>
    /// ボスが最初のレーンの位置まで来る。
    /// </summary>
    public class MoveToInitialLaneStep : BossActionStep
    {
        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;

        public MoveToInitialLaneStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            // CurrentRane.Valueの仕様で初期値が0、移動後に始めて値が代入されるため、ここは手動で合わせる。
            const int LaneIndex = 10;
            Ref.BlackBoard.CurrentLaneIndex = LaneIndex;

            _start = Ref.Body.Position;
            _end = Ref.Field.GetLanePointWithOffset(LaneIndex);
            _lerp = 0;
        }

        protected override BattleActionStep Stay()
        {
            // 移動速度
            const float Speed = 1.0f;

            Vector3 p = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.Warp(p);

            if (_lerp >= 1.0f) return Next[0];

            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * Speed;
            _lerp = Mathf.Clamp01(_lerp);

           return this;
        }
    }

    /// <summary>
    /// プレイヤーの方向を向く。
    /// </summary>
    public class LookAtPlayerStep : BossActionStep
    {
        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;

        public LookAtPlayerStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            _start = Ref.Body.Forward;
            _end = Ref.BlackBoard.PlayerDirection;
            _lerp = 0;
        }

        protected override BattleActionStep Stay()
        {
            // 振り向き速度
            const float Speed = 2.0f;

            Vector3 dir = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.LookForward(dir);

            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * Speed;
            _lerp = Mathf.Clamp01(_lerp);

            if (_lerp >= 1.0f) return Next[0];
            else return this;
        }
    }

    /// <summary>
    /// 登場演出終了。
    /// </summary>
    public class ApproachEndStep : BossActionStep
    {
        public ApproachEndStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            return this;
        }
    }
}