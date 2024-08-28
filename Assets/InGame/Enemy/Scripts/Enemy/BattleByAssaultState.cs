namespace Enemy
{
    /// <summary>
    /// 移動しつつ攻撃するステート。
    /// </summary>
    public class BattleByAssaultState : BattleState
    {
        private EnemyActionStep[] _steps;
        private BattleActionStep _currentStep;

        public BattleByAssaultState(RequiredRef requiredRef) : base(requiredRef)
        {
            _steps = new EnemyActionStep[2];
            _steps[1] = new AssaultFireStep(requiredRef, null);
            _steps[0] = new AssaultHoldStep(requiredRef, _steps[1]);

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
            foreach (EnemyActionStep s in _steps) s.Dispose();
        }
    }

    /// <summary>
    /// アサルトライフル構え。
    /// </summary>
    public class AssaultHoldStep : EnemyAttackActionStep
    {
        public AssaultHoldStep(RequiredRef requiredRef, params EnemyActionStep[] next) : base(requiredRef, next)
        {
            // アイドルのアニメーション再生をトリガーする。
            {
                string state = BodyAnimationConst.Assault.Idle;
                int layer = BodyAnimationConst.Layer.BaseLayer;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnIdleAnimationStateEnter);
            }
        }

        protected override void Enter()
        {
        }

        private void OnIdleAnimationStateEnter()
        {
            // ロケットランチャーを構える。
            Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.AttackSet);
        }

        protected override BattleActionStep Stay()
        {
            if (IsAttack()) return Next[0];
            else return this;
        }

        public override void Dispose()
        {
            Ref.BodyAnimation.ReleaseStateCallback(ID);
        }
    }

    /// <summary>
    /// アサルトライフル発射。
    /// </summary>
    public class AssaultFireStep : EnemyAttackActionStep
    {
        public AssaultFireStep(RequiredRef requiredRef, params EnemyActionStep[] next) : base(requiredRef, next)
        {
            // 発射のアニメーション再生をトリガーし、攻撃したことを黒板に書き込む。
            {
                string state = BodyAnimationConst.Assault.FireLoop;
                int layer = BodyAnimationConst.Layer.UpperBody;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnFireAnimationStateEnter);
            }
        }

        protected override void Enter()
        {
            // 1フレームズレるが、次のStayで攻撃のアニメーションが再生されるのでここでは何もしない。
        }

        private void OnFireAnimationStateEnter()
        {
            AttackTrigger();
        }

        protected override BattleActionStep Stay()
        {
            if (IsAttack())
            {
                // 黒板への書き込みはアニメーションイベントで行うので、ここでは再生だけ。
                Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.Attack);
            }

            return this;
        }

        public override void Dispose()
        {
            Ref.BodyAnimation.ReleaseStateCallback(ID);
        }
    }
}