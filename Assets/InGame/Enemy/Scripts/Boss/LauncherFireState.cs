using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss
{
    /// <summary>
    /// 戦闘中、ロケットランチャーで攻撃するステート。
    /// </summary>
    public class LauncherFireState : BattleState
    {
        // 構え->攻撃
        private BossActionStep[] _steps;
        private BattleActionStep _currentStep;

        public LauncherFireState(RequiredRef requiredRef) : base(requiredRef)
        {
            _steps = new BossActionStep[4];
            _steps[3] = new LauncherFireEndStep(requiredRef, null);
            _steps[2] = new LauncherCooldownStep(requiredRef, _steps[3]);
            _steps[1] = new LauncherFireStep(requiredRef, _steps[2]);
            _steps[0] = new LauncherHoldStep(requiredRef, _steps[1]);
        }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.LauncherFire;

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
            MoveToPointP();
            LookAtPlayer();

            _currentStep = _currentStep.Update();

            // 終了ステップまで到達したらアイドル状態に戻る。
            if (_currentStep.ID == nameof(LauncherFireEndStep)) TryChangeState(StateKey.Idle);
        }

        public override void Dispose()
        {
            foreach (BattleActionStep s in _steps) s.Dispose();
        }
    }

    /// <summary>
    /// ロケットランチャー構え。
    /// </summary>
    public class LauncherHoldStep : BossActionStep
    {
        private bool _isTransition;

        public LauncherHoldStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next)
        {
            // 構えのアニメーション再生をトリガーする。
            {
                string state = BodyAnimationConst.Boss.HoldStart;
                int layer = BodyAnimationConst.Layer.UpperBody;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnHoldAnimationStateEnter);
            }
            // Exitのコールバックが不安定なので、発射のEnterを検知して、次のステップに遷移させる用途。
            {
                string state = BodyAnimationConst.Boss.FireLoop;
                int layer = BodyAnimationConst.Layer.UpperBody;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnFireAnimationStateEnter);
            }
        }

        protected override void Enter()
        {
            Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.AttackSet);
            Ref.BodyAnimation.ResetTrigger(BodyAnimationConst.Param.BladeAttack);
        }

        private void OnHoldAnimationStateEnter()
        {
            // 現状、特にプランナーから指示が無いので構え->発射を瞬時に行う。
            Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.Attack);
        }

        private void OnFireAnimationStateEnter()
        {
            _isTransition = true;
        }

        protected override BattleActionStep Stay()
        {
            if (_isTransition) return Next[0];
            else return this;
        }

        public override void Dispose()
        {
            Ref.BodyAnimation.ReleaseStateCallback(ID);
        }
    }

    /// <summary>
    /// ロケットランチャー発射 -> リロード -> 構え を繰り返す。
    /// </summary>
    public class LauncherFireStep : BossActionStep
    {
        public LauncherFireStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            // 構えのステップで発射のアニメーション開始を検知しているので
            // このメソッドが呼ばれている時点で既に発射のアニメーションが再生開始している。
        }

        protected override BattleActionStep Stay()
        {
            BlackBoard bb = Ref.BlackBoard;

            // 近接攻撃の条件を満たした。
            bool isMelee = bb.IsWithinMeleeRange && bb.MeleeAttack.IsWaitingExecute();
            // QTE開始
            bool isQte = bb.IsQteStarted;
            // ファンネル展開
            bool isFunnel = bb.FunnelExpand.IsWaitingExecute();

            if(isMelee || isQte || isFunnel)
            {
                // 射撃のアニメーションが繰り返されるようになっているため、
                // 手動で射撃終了をトリガーする必要がある。
                Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.AttackEnd);
                
                return Next[0];
            }
            else
            {
                return this;
            }
        }
    }

    /// <summary>
    /// アニメーションの終了を待って攻撃終了させる。
    /// </summary>
    public class LauncherCooldownStep : BossActionStep
    {
        private float _timer;

        public LauncherCooldownStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            _timer = 2.0f; // アニメーションに合う用に手動で設定。
        }

        protected override BattleActionStep Stay()
        {
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _timer -= dt;
            if (_timer <= 0) return Next[0];
            else return this;
        }
    }

    /// <summary>
    /// ロケットランチャーでの攻撃終了。
    /// </summary>
    public class LauncherFireEndStep : BossActionStep
    {
        public LauncherFireEndStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            return this;
        }
    }
}
