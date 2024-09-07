using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Boss.Launcher;

namespace Enemy.Boss
{
    /// <summary>
    /// 戦闘中、ロケットランチャーで攻撃するステート。
    /// </summary>
    public class LauncherFireState : BattleState
    {
        // 構え->攻撃
        private BossActionStep[] _fireSteps;
        private BattleActionStep _currentFireStep;
        // アニメーションのWeightを調整。
        private BossActionStep[] _weightSteps;
        private BattleActionStep _currentWeightStep;

        public LauncherFireState(RequiredRef requiredRef) : base(requiredRef)
        {
            _fireSteps = new BossActionStep[4];
            _fireSteps[3] = new EndStep(requiredRef, null);
            _fireSteps[2] = new CooldownStep(requiredRef, _fireSteps[3]);
            _fireSteps[1] = new FireStep(requiredRef, _fireSteps[2]);
            _fireSteps[0] = new HoldStep(requiredRef, _fireSteps[1]);

            _weightSteps = new BossActionStep[2];
            _weightSteps[1] = new EndStep(requiredRef, null);
            _weightSteps[0] = new WeightControlStep(requiredRef, _weightSteps[1]);
        }

        protected override void OnEnter()
        {
            Ref.BlackBoard.CurrentState = StateKey.LauncherFire;

            _currentFireStep = _fireSteps[0];
            _currentWeightStep = _weightSteps[0];

            TurnToPlayer(isReset: true);
        }

        protected override void OnExit()
        {
            TurnToPlayer();
        }

        protected override void OnStay()
        {
            TurnToPlayer();
            Hovering();

            _currentFireStep = _currentFireStep.Update();
            _currentWeightStep = _currentWeightStep.Update();

            // 終了ステップまで到達したらアイドル状態に戻る。
            bool isFireEnd = _currentFireStep.ID == nameof(EndStep);
            bool isWeightControlEnd = _currentWeightStep.ID == nameof(EndStep);
            if (isFireEnd && isWeightControlEnd) TryChangeState(StateKey.Idle);
        }

        public override void Dispose()
        {
            foreach (BattleActionStep s in _fireSteps) s.Dispose();
            foreach (BattleActionStep s in _weightSteps) s.Dispose();
        }
    }
}

namespace Enemy.Boss.Launcher
{
    /// <summary>
    /// ロケットランチャー構え。
    /// </summary>
    public class HoldStep : BossActionStep
    {
        private bool _isTransition;

        public HoldStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next)
        {
            // 構えのアニメーション再生をトリガーする。
            {
                string state = Const.Boss.HoldStart;
                int layer = Const.Layer.UpperBody;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnHoldAnimationStateEnter);
            }
            // Exitのコールバックが不安定なので、発射のEnterを検知して、次のステップに遷移させる用途。
            {
                string state = Const.Boss.FireLoop;
                int layer = Const.Layer.UpperBody;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnFireAnimationStateEnter);
            }
        }

        protected override void Enter()
        {
            Ref.BodyAnimation.SetTrigger(Const.Param.AttackSet);
            Ref.BodyAnimation.ResetTrigger(Const.Param.BladeAttack);
        }

        private void OnHoldAnimationStateEnter()
        {
            // 現状、特にプランナーから指示が無いので構え->発射を瞬時に行う。
            Ref.BodyAnimation.SetTrigger(Const.Param.Attack);
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
    /// ロケットランチャー発射をn回繰り返す。
    /// </summary>
    public class FireStep : BossActionStep
    {
        private int _count;

        public FireStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next)
        {
            // リロードのアニメーション再生をトリガーする。
            string state = Const.Boss.Reload;
            int layer = Const.Layer.UpperBody;
            Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnReloadAnimationStateEnter);
        }

        private void OnReloadAnimationStateEnter()
        {
            _count--;
            _count = Mathf.Max(0, _count);
        }

        protected override void Enter()
        {
            // 構えのステップで発射のアニメーション開始を検知しているので
            // このメソッドが呼ばれている時点で既に発射のアニメーションが再生開始している。
            // 発射->このステップに遷移 は瞬時に行われるが、リロードの再生までは1秒程度余裕がある。
            // そのため、ここで発射カウントをリセットしても正常に動く。
            int max = Ref.BossParams.RangeAttackConfig.MaxContinuous;
            int min = Ref.BossParams.RangeAttackConfig.MinContinuous;
            _count = Random.Range(min, max + 1);
        }

        protected override BattleActionStep Stay()
        {
            if (_count <= 0)
            {
                // 射撃のアニメーションが繰り返されるようになっているため、
                // 手動で射撃終了をトリガーする必要がある。
                // リロードのアニメーション再生開始のコールバックで発射カウントを減らしているので
                // 次のステップに遷移するタイミングはリロードのアニメーション再生開始とほぼ同じ。
                Ref.BodyAnimation.SetTrigger(Const.Param.AttackEnd);

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
    public class CooldownStep : BossActionStep
    {
        private float _timer;

        public CooldownStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            _timer = 3.0f; // アニメーションに合う用に手動で設定。
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
    /// アニメーションに応じたWeightの調整。
    /// </summary>
    public class WeightControlStep : BossActionStep
    {
        private float _weight;
        private int _sign;
        // Weightの値だけで遷移の判定をすると挙動が怪しいので、一度構えた状態になったフラグ。
        private bool _isHoldExecuted;

        public WeightControlStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next)
        {
            // 構えのアニメーション再生をトリガーする。
            {
                string state = Const.Boss.HoldStart;
                int layer = Const.Layer.UpperBody;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnHoldAnimationStateEnter);
            }
            // アイドルのアニメーション再生をトリガーする。
            {
                string state = Const.Boss.Idle;
                int layer = Const.Layer.UpperBody;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnIdleAnimationStateEnter);
            }
        }

        private void OnHoldAnimationStateEnter()
        {
            // 構えたタイミングでWeightを徐々に上げていく。
            _sign = 1;
            _isHoldExecuted = true;
        }

        private void OnIdleAnimationStateEnter()
        {
            // アイドルに戻ったタイミングで徐々にWeightを下げていく。
            _sign = -1;
        }

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            // 変化速度。
            const float Speed = 2.0f;

            float dt = Ref.BlackBoard.PausableDeltaTime;
            _weight += dt * _sign * Speed;
            _weight = Mathf.Clamp01(_weight);

            Ref.BodyAnimation.SetUpperBodyWeight(_weight);

            // 1度構えた状態になってアイドルに戻ったかつ、重みが0になった場合。
            if (_isHoldExecuted && _weight <= 0) return Next[0];
            else return this;
        }
    }

    /// <summary>
    /// ロケットランチャーでの攻撃終了。
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