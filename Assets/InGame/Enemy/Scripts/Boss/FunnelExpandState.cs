using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Funnel;
using Enemy.Boss.FunnelExpand;

namespace Enemy.Boss
{
    /// <summary>
    /// 戦闘中、ファンネルを展開するステート。
    /// </summary>
    public class FunnelExpandState : BattleState
    {
        // プレイヤーの正面のレーンに移動し展開、展開後はプレイヤーの入力があるまで待機。
        private BossActionStep[] _steps;
        private BattleActionStep _currentStep;

        public FunnelExpandState(RequiredRef requiredRef) : base(requiredRef)
        {
            _steps = new BossActionStep[5];
            _steps[4] = new EndStep(requiredRef, null);
            _steps[3] = new WaitPlayerInputStep(requiredRef, _steps[4]);
            _steps[2] = new ExpandStep(requiredRef, _steps[3]);
            _steps[1] = new WaitAnimationStep(requiredRef, _steps[2]);
            _steps[0] = new LaneChangeStep(requiredRef, _steps[1]);
        }

        protected override void OnEnter()
        {
            Ref.BlackBoard.CurrentState = StateKey.FunnelExpand;

            _currentStep = _steps[0];
        }

        protected override void OnExit()
        {
            // 展開後、プレイヤーの入力があり、敵が動き出すタイミングで実行を黒板に書き込む。
            Ref.BlackBoard.FunnelExpand.Execute();
        }

        protected override void OnStay()
        {
            _currentStep = _currentStep.Update();

            if (_currentStep.ID == nameof(EndStep)) TryChangeState(StateKey.Idle);
        }
    }
}

namespace Enemy.Boss.FunnelExpand
{
    /// <summary>
    /// レーン移動。
    /// </summary>
    public class LaneChangeStep : LaneChange.LaneChangeStep
    {
        public LaneChangeStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next)
        {
            // 現状、LaneChangeStateのものと全く同じ。
        }
    }

    /// <summary>
    /// レーン移動と並行してプレイヤーを向く。
    /// </summary>
    public class WaitAnimationStep : LaneChange.WaitAnimationStep
    {
        public WaitAnimationStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next)
        {
            // 現状、LaneChangeStateのものと全く同じ。
        }
    }

    /// <summary>
    /// その場でファンネルを展開。
    /// </summary>
    public class ExpandStep : BossActionStep
    {
        public ExpandStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            Ref.BodyAnimation.SetTrigger(Const.Param.FunnelExpand);

            // ファンネル展開
            foreach (FunnelController f in Ref.Funnels) f.Expand();

            Vector3 p = Ref.Body.Position;
            AudioWrapper.PlaySE(p, "SE_Funnel");
        }

        protected override BattleActionStep Stay()
        {
            return Next[0];
        }
    }

    /// <summary>
    /// プレイヤーの入力を待つ間は攻撃しない。
    /// </summary>
    public class WaitPlayerInputStep : BossActionStep
    {
        public WaitPlayerInputStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {

        }

        protected override BattleActionStep Stay()
        {
            Trigger resume = Ref.BlackBoard.ResumeBossAction;

            if (resume.IsWaitingExecute())
            {
                resume.Execute();

                // 行動再開と同時にファンネルが攻撃してくる。
                foreach (FunnelController f in Ref.Funnels) f.FireEnable(true);

                return Next[0];
            }
            else return this;
        }
    }

    /// <summary>
    /// ファンネル展開終了。
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