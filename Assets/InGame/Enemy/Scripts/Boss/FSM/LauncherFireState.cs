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
        // 構え->攻撃
        private BattleActionStep _launcherHold;
        private BattleActionStep _launcherFire;
        private BattleActionStep _launcherCooldown;
        private BattleActionStep _launcherFireEnd;

        private BattleActionStep _currentStep;

        public LauncherFireState(StateRequiredRef requiredRef) : base(requiredRef)
        {
            _launcherFireEnd = new LauncherFireEndStep();
            _launcherCooldown = new LauncherCooldownStep(requiredRef, _launcherFireEnd);
            _launcherFire = new LauncherFireStep(requiredRef, _launcherCooldown);
            _launcherHold = new LauncherHoldStep(requiredRef, _launcherFire);
        }

        protected override void Enter()
        {
            _blackBoard.CurrentState = StateKey.LauncherFire;
            _currentStep = _launcherHold;
        }

        protected override void Exit()
        {
            _launcherHold.Reset();
            _launcherFire.Reset();
            _launcherCooldown.Reset();
            _launcherFireEnd.Reset();
        }

        protected override void Stay()
        {
            PlayDamageSE();
            FunnelExpand();
            FunnelLaserSight();
            MoveToPointP();
            LookAtPlayer();

            _currentStep = _currentStep.Update();

            // 終了ステップまで到達したらアイドル状態に戻る。
            if (_currentStep.ID == nameof(LauncherFireEndStep)) TryChangeState(StateKey.Idle);
        }

        public override void Dispose()
        {
            // コールバックをステップごとに登録しているので登録解除させる。
            _launcherHold.Dispose();
            _launcherFire.Dispose();
        }
    }

    /// <summary>
    /// ロケットランチャー構え。
    /// </summary>
    public class LauncherHoldStep : BattleActionStep
    {
        private BodyAnimation _animation;
        private BattleActionStep _fire;

        private bool _isTransition;

        public LauncherHoldStep(StateRequiredRef requiredRef, BattleActionStep fire)
        {
            _animation = requiredRef.BodyAnimation;
            _fire = fire;

            // 構えのアニメーション再生をトリガーする。
            {
                string state = BodyAnimationConst.Boss.HoldStart;
                int layer = BodyAnimationConst.Layer.UpperBody;
                _animation.RegisterStateEnterCallback(ID, state, layer, OnHoldAnimationStateEnter);
            }
            // Exitのコールバックが不安定なので、発射のEnterを検知して、次のステップに遷移させる用途。
            {
                string state = BodyAnimationConst.Boss.FireLoop;
                int layer = BodyAnimationConst.Layer.UpperBody;
                _animation.RegisterStateEnterCallback(ID, state, layer, OnFireAnimationStateEnter);
            }
        }

        public override string ID => nameof(LauncherHoldStep);

        protected override void Enter()
        {
            _animation.SetTrigger(BodyAnimationConst.Param.AttackSetTrigger);
            _animation.ResetTrigger(BodyAnimationConst.Param.BladeAttackTrigger);
        }

        private void OnHoldAnimationStateEnter()
        {
            // 現状、特にプランナーから指示が無いので構え->発射を瞬時に行う。
            _animation.SetTrigger(BodyAnimationConst.Param.AttackTrigger);
        }

        private void OnFireAnimationStateEnter()
        {
            _isTransition = true;
        }

        protected override BattleActionStep Stay()
        {
            if (_isTransition) return _fire;
            else return this;
        }

        public override void Dispose()
        {
            _animation.ReleaseStateCallback(ID);
        }
    }

    /// <summary>
    /// ロケットランチャー発射。
    /// </summary>
    public class LauncherFireStep : BattleActionStep
    {
        private BlackBoard _blackBoard;
        private BodyAnimation _animation;
        private BattleActionStep _cooldown;

        public LauncherFireStep(StateRequiredRef requiredRef, BattleActionStep cooldown)
        {
            _blackBoard = requiredRef.BlackBoard;
            _animation = requiredRef.BodyAnimation;
            _cooldown = cooldown;
        }

        public override string ID => nameof(LauncherFireStep);

        protected override void Enter()
        {
            // 構えのステップで発射のアニメーション開始を検知しているので
            // このメソッドが呼ばれている時点で既に発射のアニメーションが再生開始している。
        }

        protected override BattleActionStep Stay()
        {
            // 近接攻撃の条件を満たした。
            bool isMelee = _blackBoard.IsWithinMeleeRange && _blackBoard.MeleeAttack == Trigger.Ordered;
            // QTE開始
            bool isQte = _blackBoard.IsQteEventStarted;

            if(isMelee || isQte)
            {
                // 射撃のアニメーションが繰り返されるようになっているため、
                // 手動で射撃終了をトリガーする必要がある。
                _animation.SetTrigger(BodyAnimationConst.Param.AttackEndTrigger);
                
                return _cooldown;
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
    public class LauncherCooldownStep : BattleActionStep
    {
        private BlackBoard _blackBoard;
        private BattleActionStep _fireEnd;

        private float _timer;

        public LauncherCooldownStep(StateRequiredRef requiredRef, BattleActionStep fireEnd)
        {
            _blackBoard = requiredRef.BlackBoard;
            _fireEnd = fireEnd;
        }

        public override string ID => nameof(LauncherCooldownStep);

        protected override void Enter()
        {
            _timer = 2.0f; // アニメーションに合う用に手動で設定。
        }

        protected override BattleActionStep Stay()
        {
            _timer -= _blackBoard.PausableDeltaTime;
            if (_timer <= 0) return _fireEnd;
            else return this;
        }
    }

    /// <summary>
    /// ロケットランチャーでの攻撃終了。
    /// </summary>
    public class LauncherFireEndStep : BattleActionStep
    {
        public override string ID => nameof(LauncherFireEndStep);

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            return this;
        }
    }
}
