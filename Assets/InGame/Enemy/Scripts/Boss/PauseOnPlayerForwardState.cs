using Enemy.Boss.PauseOnPlayerForward;
using Enemy.Funnel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss
{
    /// <summary>
    /// ファンネルのマルチロックオン時にプレイヤーの正面に移動させ、待機。
    /// </summary>
    public class PauseOnPlayerForwardState : BattleState
    {
        // プレイヤーの正面のレーンに移動し入力があるまで待機。
        private BossActionStep[] _steps;
        private BattleActionStep _currentStep;

        public PauseOnPlayerForwardState(RequiredRef requiredRef) : base(requiredRef)
        {
            _steps = new BossActionStep[4];
            _steps[3] = new EndStep(requiredRef, null);
            _steps[2] = new WaitPlayerInputStep(requiredRef, _steps[3]);
            _steps[1] = new WaitAnimationStep(requiredRef, _steps[2]);
            _steps[0] = new LaneChangeStep(requiredRef, _steps[1]);
        }

        protected override void OnEnter()
        {
            Ref.BlackBoard.CurrentState = StateKey.PauseOnPlayerForward;

            _currentStep = _steps[0];

            TurnToPlayer(isReset: true);

            Ref.BlackBoard.PauseOnPlayerForward.Execute();
        }

        protected override void OnExit()
        {
            TurnToPlayer();
        }

        protected override void OnStay()
        {
            TurnToPlayer();

            _currentStep = _currentStep.Update();

            if (_currentStep.ID == nameof(EndStep)) TryChangeState(StateKey.Idle);
        }
    }
}

namespace Enemy.Boss.PauseOnPlayerForward
{
    /// <summary>
    /// プレイヤーの正面レーンに移動。
    /// </summary>
    public class LaneChangeStep : LaneChange.LaneChangeStep
    {
        public LaneChangeStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next)
        {
            // レーンチェンジのものと全く同じ。
        }
    }

    /// <summary>
    /// 左右移動のアニメーションを待つ。
    /// </summary>
    public class WaitAnimationStep : LaneChange.WaitAnimationStep
    {
        public WaitAnimationStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next)
        {
            // レーンチェンジのものと全く同じ。
        }
    }

    /// <summary>
    /// 行動再開まで何もしない。
    /// </summary>
    public class WaitPlayerInputStep : BossActionStep
    {
        public WaitPlayerInputStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            foreach (FunnelController f in Ref.Funnels)
            {
                f.FireEnable(false);
            }
        }

        protected override BattleActionStep Stay()
        {
            Trigger resume = Ref.BlackBoard.ResumeBossAction;

            if (resume.IsWaitingExecute())
            {
                resume.Execute();

                foreach (FunnelController f in Ref.Funnels)
                {
                    f.FireEnable(true);
                }

                return Next[0];
            }
            else return this;
        }
    }

    /// <summary>
    /// ポーズ終了。
    /// </summary>
    public class EndStep : BossActionStep
    {
        public EndStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            return this;
        }
    }
}