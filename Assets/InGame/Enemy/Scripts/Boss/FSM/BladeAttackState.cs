using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss.FSM
{
    /// <summary>
    /// 戦闘中、刀で攻撃するステート。
    /// </summary>
    public class BladeAttackState : BattleState
    {
        private enum Step { Other };

        // チャージ->接近->下段斬りor溜め突き->自機とすれ違う
        ChargeJetPackStep _chargeJetPack;
        ApproachToPlayerStep _approachToPlayer;
        GedanGiriStep _gedanGiri;
        ChargeThrustStep _chargeThrust;
        PassingStep _passing;

        Step _currentStep;

        public BladeAttackState(StateRequiredRef requiredRef) : base(requiredRef)
        {
            _chargeJetPack = new ChargeJetPackStep(requiredRef);
            _approachToPlayer= new ApproachToPlayerStep(requiredRef);
            _gedanGiri = new GedanGiriStep(requiredRef);
            _chargeThrust = new ChargeThrustStep(requiredRef);
            _passing = new PassingStep(requiredRef);
        }

        protected override void Enter()
        {
            _blackBoard.CurrentState = StateKey.BladeAttack;

            //_animation.SetTrigger(近接攻撃構えのトリガー名);
            _animation.ResetTrigger(BodyAnimationConst.Param.AttackSetTrigger);
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            PlayDamageSE();
            FunnelExpand();
            FunnelLaserSight();

            // アニメーションが無いので攻撃できない。
            TryChangeState(StateKey.Idle);
        }

        public override void Dispose()
        {
            // コールバックの登録解除ｺｺ
        }
    }

    /// <summary>
    /// ジェットパックが光ってチャージ。
    /// </summary>
    public class ChargeJetPackStep : BattleActionStep
    {
        BodyAnimation _animation;

        public ChargeJetPackStep(StateRequiredRef requiredRef)
        {
            _animation = requiredRef.BodyAnimation;
        }

        protected override void Enter()
        {
        }

        protected override void Stay()
        {
        }
    }

    /// <summary>
    /// 自機に接近。
    /// </summary>
    public class ApproachToPlayerStep : BattleActionStep
    {
        BodyAnimation _animation;

        public ApproachToPlayerStep(StateRequiredRef requiredRef)
        {
            _animation = requiredRef.BodyAnimation;
        }

        protected override void Enter()
        {
        }

        protected override void Stay()
        {
        }
    }

    /// <summary>
    /// 下段斬り。
    /// </summary>
    public class GedanGiriStep : BattleActionStep
    {
        BodyAnimation _animation;

        public GedanGiriStep(StateRequiredRef requiredRef)
        {
            _animation = requiredRef.BodyAnimation;
        }

        protected override void Enter()
        {
        }

        protected override void Stay()
        {
        }
    }

    /// <summary>
    /// 溜めからの突き。
    /// </summary>
    public class ChargeThrustStep : BattleActionStep
    {
        BodyAnimation _animation;

        public ChargeThrustStep(StateRequiredRef requiredRef)
        {
            _animation = requiredRef.BodyAnimation;
        }

        protected override void Enter()
        {
        }

        protected override void Stay()
        {
        }
    }

    /// <summary>
    /// 自機とすれ違う。
    /// </summary>
    public class PassingStep : BattleActionStep
    {
        BodyAnimation _animation;

        public PassingStep(StateRequiredRef requiredRef)
        {
            _animation = requiredRef.BodyAnimation;
        }

        protected override void Enter()
        {
        }

        protected override void Stay()
        {
        }
    }
}
