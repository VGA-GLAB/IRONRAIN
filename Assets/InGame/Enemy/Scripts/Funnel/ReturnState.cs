using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Funnel.Return;

namespace Enemy.Funnel
{
    public class ReturnState : State<StateKey>
    {
        private BattleActionStep[] _steps;
        private BattleActionStep _currentStep;

        public ReturnState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;

            _steps = new BattleActionStep[3];
            _steps[2] = new EndStep(requiredRef, null);
            _steps[1] = new ReturnStep(requiredRef, _steps[2]);
            _steps[0] = new KnockbackStep(requiredRef, _steps[1]);
        }

        protected RequiredRef Ref { get; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Return;

            AgentScript agent = Ref.AgentScript;
            if (agent != null) agent.EnemyDestory();

            _currentStep = _steps[0];
        }

        protected override void Exit()
        {
            int index = Ref.BlackBoard.FlySeIndex;
            AudioWrapper.StopSE(index);
        }

        protected override void Stay()
        {
            _currentStep = _currentStep.Update();

            if (_currentStep.ID == nameof(EndStep)) TryChangeState(StateKey.Hide);
        }
    }
}

namespace Enemy.Funnel.Return
{
    /// <summary>
    /// 後ろにノックバックする。
    /// </summary>
    public class KnockbackStep : BattleActionStep
    {
        private Vector3 _velocity;

        public KnockbackStep(RequiredRef requiredRef, BattleActionStep next) : base(next)
        {
            Ref = requiredRef;
        }

        RequiredRef Ref { get; }

        protected override void Enter()
        {
            // ふっ飛ばし力
            const float MinPower = 1.5f;
            const float MaxPower = 2.0f;
            const float RandomRate = 0.1f;

            // プレイヤーの攻撃を受けた反対方向に吹っ飛ばす。
            Vector3 pd = Ref.BlackBoard.PlayerDirection;
            pd.x += Random.Range(-RandomRate, RandomRate);
            pd.y += Random.Range(-RandomRate, RandomRate);
            pd.z += Random.Range(-RandomRate, RandomRate);
            float power = Random.Range(MinPower, MaxPower);
            _velocity = -pd * power;
        }

        protected override BattleActionStep Stay()
        {
            // DeltaTimeをそのままかけると値が小さくなりすぎてしまうので1から引いた値を使う。
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _velocity *= 0.98f * (1 - dt);
            Ref.Body.Move(_velocity);

            // ある程度減速したらノックバック終了、停止したとみなす。
            const float StopThreshold = 1.0f;
            if (_velocity.sqrMagnitude < StopThreshold) return Next[0];
            else return this;
        }
    }

    /// <summary>
    /// ボス本体に戻る。
    /// </summary>
    public class ReturnStep : BattleActionStep
    {
        // 到達までのおおよその時間
        const float SmoothTime = 0.5f;
        // 到達後、このステップを抜けるまでのディレイ
        const float Delay = 1.0f;

        private Vector3 _velocity;
        private float _elapsed = 0;

        public ReturnStep(RequiredRef requiredRef, BattleActionStep next) : base(next)
        {
            Ref = requiredRef;
        }

        RequiredRef Ref { get; }

        protected override void Enter()
        {
            _velocity = Vector3.zero;
            _elapsed = 0;
        }

        protected override BattleActionStep Stay()
        {
            if (_elapsed > SmoothTime + Delay)
            {
                return Next[0];
            }
            else 
            {
                float dt = Ref.BlackBoard.PausableDeltaTime;
                _elapsed += dt;

                Vector3 p = Ref.Body.Position;
                Vector3 bp = Ref.Boss.transform.position;
                Vector3 warp = Vector3.SmoothDamp(p, bp, ref _velocity, SmoothTime);
                Ref.Body.Warp(warp);

                // ボスの方へ振り向く速さ。
                const float LookSpeed = 3.0f;

                Vector3 bd = Ref.BlackBoard.BossDirection;
                Vector3 fwd = Ref.Body.Forward;
                Vector3 look = Vector3.Lerp(fwd, bd, dt * LookSpeed);
                Ref.Body.LookForward(look);

                return this;
            }
        }
    }

    /// <summary>
    /// 終了。
    /// </summary>
    public class EndStep : BattleActionStep
    {
        public EndStep(RequiredRef _, BattleActionStep next) : base(next)
        {
        }

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            return this;
        }
    }
}
