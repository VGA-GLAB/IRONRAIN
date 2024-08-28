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
            _steps[0] = new ShieldHoldStep(requiredRef, null);

            _currentStep = _steps[0];
        }

        protected override void OnEnter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Battle;
        }

        protected override void OnExit()
        {
            // 死亡と撤退どちらの場合でも、武器を下ろすアニメーションをトリガー。
            Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.AttackEnd);
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

    /// <summary>
    /// 盾を構える。
    /// </summary>
    public class ShieldHoldStep : EnemyActionStep
    {
        public ShieldHoldStep(RequiredRef requiredRef, params EnemyActionStep[] next) : base(requiredRef, next)
        {
            // アイドルのアニメーション再生をトリガーする。
            {
                string state = BodyAnimationConst.Shield.Idle;
                int layer = BodyAnimationConst.Layer.BaseLayer;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnIdleAnimationStateEnter);
            }
        }

        protected override void Enter()
        {
        }

        private void OnIdleAnimationStateEnter()
        {
            // 盾を構える。
            Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.AttackSet);
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