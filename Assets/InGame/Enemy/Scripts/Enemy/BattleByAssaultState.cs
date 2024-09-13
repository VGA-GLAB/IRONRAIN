using UnityEngine;
using Enemy.Assault;

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
            _steps[1] = new FireStep(requiredRef, null);
            _steps[0] = new HoldStep(requiredRef, _steps[1]);

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
            foreach (EnemyActionStep s in _steps) s.Dispose();
        }
    }
}

namespace Enemy.Assault
{
    /// <summary>
    /// アサルトライフル構え。
    /// </summary>
    public class HoldStep : EnemyAttackActionStep
    {
        // UpperBodyのWeight。
        private float _weight;

        public HoldStep(RequiredRef requiredRef, params EnemyActionStep[] next) : base(requiredRef, next)
        {
            // アイドルのアニメーション再生をトリガーする。
            {
                string state = Const.Assault.Idle;
                int layer = Const.Layer.BaseLayer;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnIdleAnimationStateEnter);
            }

            // このステップは繰り返されるのでEnterではなく、コンストラクタで初期化。
            // 最初の1回目の構えの時にのみ、Weightを変更する。
            _weight = 0;
        }

        protected override void Enter()
        {
        }

        private void OnIdleAnimationStateEnter()
        {
            // アサルトライフルを構える。
            Ref.BodyAnimation.SetTrigger(Const.Param.AttackSet);
        }

        protected override BattleActionStep Stay()
        {
            // UpperBodyのWeightを0から1にしてから次のステップに遷移。
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _weight += dt;
            _weight = Mathf.Clamp01(_weight);
            Ref.BodyAnimation.SetUpperBodyWeight(_weight);

            if (_weight >= 1.0f && IsAttack()) return Next[0];
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
    public class FireStep : EnemyAttackActionStep
    {
        public FireStep(RequiredRef requiredRef, params EnemyActionStep[] next) : base(requiredRef, next)
        {
            // 発射のアニメーション再生をトリガーし、攻撃したことを黒板に書き込む。
            {
                string state = Const.Assault.FireLoop;
                int layer = Const.Layer.UpperBody;
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
                Ref.BodyAnimation.SetTrigger(Const.Param.Attack);
            }

            return this;
        }

        public override void Dispose()
        {
            Ref.BodyAnimation.ReleaseStateCallback(ID);
        }
    }
}