using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss.FSM
{
    /// <summary>
    /// 戦闘中、ロケットランチャーで攻撃するステート。
    /// </summary>
    public class LauncherFireState : BattleState
    {
        private enum Step { Other, Hold, Fire };

        // 構え->攻撃
        LauncherHoldStep _launcherHold;
        LauncherFireStep _launcherFire;

        Step _currentStep;

        public LauncherFireState(StateRequiredRef requiredRef) : base(requiredRef)
        {
            _launcherHold = new LauncherHoldStep(requiredRef);
            _launcherFire = new LauncherFireStep(requiredRef);

            _animation.RegisterStateEnterCallback(
                nameof(LauncherFireState), 
                BodyAnimationConst.Boss.HoldStart, 
                BodyAnimationConst.Layer.UpperBody, 
                () => _currentStep = Step.Hold
                );

            _animation.RegisterStateEnterCallback(
                nameof(LauncherFireState),
                BodyAnimationConst.Boss.FireLoop,
                BodyAnimationConst.Layer.UpperBody,
                () => _currentStep = Step.Fire
                );
        }

        protected override void Enter()
        {
            _animation.SetTrigger(BodyAnimationConst.Param.AttackSetTrigger);
            // _animation.ResetTrigger(近接攻撃構えのトリガー名);
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            PlayDamageSE();
            FunnelExpand();
            MoveToPointP();
            LookAtPlayer();

            if (_currentStep == Step.Hold) _launcherHold.Update();
            else if (_currentStep == Step.Fire) _launcherFire.Update();
        }

        public override void Dispose()
        {
            // コールバックの登録解除。
            _animation.ReleaseStateCallback(nameof(LauncherFireState));
        }
    }

    /// <summary>
    /// ロケットランチャー構え。
    /// </summary>
    public class LauncherHoldStep : BattleActionStep
    {
        BodyAnimation _animation;

        public LauncherHoldStep(StateRequiredRef requiredRef)
        {
            _animation = requiredRef.BodyAnimation;
        }

        protected override void Enter()
        {
        }

        protected override void Stay()
        {
            // 現状、特にプランナーから指示が無いので構え->発射を瞬時に行う。
            _animation.SetTrigger(BodyAnimationConst.Param.AttackTrigger);
        }
    }

    /// <summary>
    /// ロケットランチャー発射。
    /// </summary>
    public class LauncherFireStep : BattleActionStep
    {
        BlackBoard _blackBoard;
        BodyAnimation _animation;

        public LauncherFireStep(StateRequiredRef requiredRef)
        {
            _blackBoard = requiredRef.BlackBoard;
            _animation = requiredRef.BodyAnimation;
        }

        protected override void Enter()
        {
        }

        protected override void Stay()
        {
            // 射撃のアニメーションのステートが繰り返されるようになっているため、
            // 手動で射撃終了をトリガーしないと近接攻撃出来ない。
            if (_blackBoard.IsWithinMeleeRange && _blackBoard.NextMeleeAttackTime < Time.time)
            {
                _animation.SetTrigger(BodyAnimationConst.Param.AttackEndTrigger);
            }
        }
    }
}
