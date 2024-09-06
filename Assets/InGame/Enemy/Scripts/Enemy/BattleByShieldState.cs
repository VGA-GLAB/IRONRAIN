using UnityEngine;
using Enemy.Shield;

namespace Enemy
{
    /// <summary>
    /// 移動しつつ攻撃するステート。
    /// </summary>
    public class BattleByShieldState : BattleState
    {
        // 構える。
        private EnemyActionStep[] _steps;
        private BattleActionStep _currentStep;

        public BattleByShieldState(RequiredRef requiredRef) : base(requiredRef)
        {
            _steps = new EnemyActionStep[1];
            _steps[0] = new HoldStep(requiredRef, null);

            _currentStep = _steps[0];
        }

        protected override void OnEnter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Battle;
        }

        protected override void OnExit()
        {
            // 死亡と撤退どちらの場合でも、武器を下ろす。
            Ref.BodyAnimation.SetTrigger(Const.Param.AttackEnd);
            Ref.BodyAnimation.SetUpperBodyWeight(0);
        }

        protected override void StayIfBattle()
        {
            _currentStep = _currentStep.Update();
        }

        public override void Dispose()
        {
            foreach (BattleActionStep s in _steps) s.Dispose();
        }
    }    
}

namespace Enemy.Shield
{
    /// <summary>
    /// 盾を構える。
    /// </summary>
    public class HoldStep : EnemyActionStep
    {
        public HoldStep(RequiredRef requiredRef, params EnemyActionStep[] next) : base(requiredRef, next)
        {
            // アイドルのアニメーション再生をトリガーする。
            {
                string state = Const.Shield.Idle;
                int layer = Const.Layer.BaseLayer;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnIdleAnimationStateEnter);
            }

            // 構えのアニメーション再生をトリガーする。
            {
                string state = Const.Shield.ShieldLoop;
                int layer = Const.Layer.BaseLayer;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnHoldAnimationStateEnter);
            }
        }

        protected override void Enter()
        {
        }

        private void OnIdleAnimationStateEnter()
        {
            // 盾を構える。
            Ref.BodyAnimation.SetTrigger(Const.Param.AttackSet);
        }

        private void OnHoldAnimationStateEnter()
        {
            // 攻撃する処理は特にないので、盾構え後そのまま攻撃のアニメーション再生するだけ。
            Ref.BodyAnimation.SetTrigger(Const.Param.Attack);
        }

        protected override BattleActionStep Stay()
        {
            return this;
        }

        public override void Dispose()
        {
            Ref.BodyAnimation.ReleaseStateCallback(ID);
        }
    }
}