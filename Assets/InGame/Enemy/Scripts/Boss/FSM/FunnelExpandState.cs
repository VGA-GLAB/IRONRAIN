using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Funnel;

namespace Enemy.Boss.FSM
{
    /// <summary>
    /// 戦闘中、ファンネルを展開するステート。
    /// </summary>
    public class FunnelExpandState : BattleState
    {
        // アニメーションの再生と同時に展開後、一定時間待つ。
        private BattleActionStep[] _steps;
        private BattleActionStep _currentStep;

        public FunnelExpandState(RequiredRef requiredRef) : base(requiredRef)
        {
            _steps = new BattleActionStep[2];
            _steps[1] = new FunnelExpandEndStep(requiredRef, null);
            _steps[0] = new FunnelExpandStep(requiredRef, _steps[1]);
        }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.FunnelExpand;

            _currentStep = _steps[0];
        }

        protected override void Exit()
        {
            foreach (BattleActionStep s in _steps) s.Reset();
        }

        protected override void Stay()
        {
            PlayDamageSE();
            FunnelLaserSight();

            _currentStep = _currentStep.Update();

            // 終了ステップまで到達したらアイドル状態に戻る。
            if (_currentStep.ID == nameof(FunnelExpandEndStep)) TryChangeState(StateKey.Idle);
        }
    }

    /// <summary>
    /// その場でファンネルを展開。
    /// </summary>
    public class FunnelExpandStep : BattleActionStep
    {
        private float _timer;

        public FunnelExpandStep(RequiredRef requiredRef, BattleActionStep next) : base(requiredRef, next) { }

        public override string ID => nameof(FunnelExpandStep);

        protected override void Enter()
        {
            _timer = 2.0f; // アニメーションの再生時間に手動で合わせる。
            Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.FunnelExpand);
            
            // ファンネル展開
            foreach (FunnelController f in Ref.Funnels) f.Expand();
            AudioWrapper.PlaySE("SE_Funnel");

            // このタイミングで黒板に実行を書き込んでいるが、ステートのExitでも良いかも？
            Ref.BlackBoard.FunnelExpand = Trigger.Executed;
        }

        protected override BattleActionStep Stay()
        {
            // 時間が来たら遷移。
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _timer -= dt;
            if (_timer > 0) return this;
            else return Next[0];
        }
    }

    /// <summary>
    /// ファンネル展開終了。
    /// </summary>
    public class FunnelExpandEndStep : BattleActionStep
    {
        public FunnelExpandEndStep(RequiredRef requiredRef, BattleActionStep next) : base(requiredRef, next) { }

        public override string ID => nameof(FunnelExpandEndStep);

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            return this;
        }
    }
}
