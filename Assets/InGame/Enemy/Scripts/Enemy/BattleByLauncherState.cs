using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 移動しつつ攻撃するステート。
    /// </summary>
    public class BattleByLauncherState : BattleState
    {
        private EnemyActionStep[] _steps;
        private BattleActionStep _currentStep;

        public BattleByLauncherState(RequiredRef requiredRef) : base(requiredRef)
        {
            _steps = new EnemyActionStep[3];
            _steps[2] = new LauncherEndStep(requiredRef);
            _steps[1] = new LauncherFireStep(requiredRef, _steps[2]);
            _steps[0] = new LauncherHoldStep(requiredRef, _steps[1]);
            // 構えと攻撃を繰り返すので、コンストラクタではなく、メソッドから遷移先を追加。
            _steps[2].AddNext(_steps[0]);

            _currentStep = _steps[0];
        }

        protected override void OnEnter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Battle;
        }

        protected override void OnExit()
        {
            // 死亡と撤退どちらの場合でも、武器を下ろすアニメーションをトリガー。
            Ref.BodyAnimation.SetTrigger(Const.Param.AttackEnd);
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
    /// ロケットランチャー構え。
    /// </summary>
    public class LauncherHoldStep : EnemyAttackActionStep
    {
        public LauncherHoldStep(RequiredRef requiredRef, params EnemyActionStep[] next) : base(requiredRef, next)
        {
            // アイドルのアニメーション再生をトリガーする。
            {
                string state = Const.Launcher.Idle;
                int layer = Const.Layer.BaseLayer;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnIdleAnimationStateEnter);
            }
        }

        protected override void Enter()
        {
        }

        private void OnIdleAnimationStateEnter()
        {
            // ロケットランチャーを構える。
            Ref.BodyAnimation.SetTrigger(Const.Param.AttackSet);
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
    /// ロケットランチャー発射。
    /// </summary>
    public class LauncherFireStep : EnemyAttackActionStep
    {
        // アニメーションの再生をトリガーと同時に再生されるとは限らないので、一応フラグ。
        private bool _isAnimationEnter;

        public LauncherFireStep(RequiredRef requiredRef, params EnemyActionStep[] next) : base(requiredRef, next)
        {
            // 発射のアニメーション再生をトリガーし、攻撃したことを黒板に書き込む。
            {
                string state = Const.Launcher.Fire;
                int layer = Const.Layer.UpperBody;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnFireAnimationStateEnter);
            }
        }

        protected override void Enter()
        {
            _isAnimationEnter = false;

            // ロケットランチャー発射。
            Ref.BodyAnimation.SetTrigger(Const.Param.Attack);
        }

        private void OnFireAnimationStateEnter()
        {
            AttackTrigger();
            _isAnimationEnter = true;
        }

        protected override BattleActionStep Stay()
        {
            // 発射後ではなく、発射のアニメーションを再生した瞬間、次に遷移するので注意。
            if (_isAnimationEnter) return Next[0];
            else return this;
        }

        public override void Dispose()
        {
            Ref.BodyAnimation.ReleaseStateCallback(ID);
        }
    }

    /// <summary>
    /// ロケットランチャー発射終了、構えに戻す。
    /// </summary>
    public class LauncherEndStep : EnemyAttackActionStep
    {
        public LauncherEndStep(RequiredRef _, params EnemyActionStep[] next) : base(_, next)
        {
        }

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            return Next[0];
        }

        public override void Dispose()
        {
        }
    }
}