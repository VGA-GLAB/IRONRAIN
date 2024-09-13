using UnityEngine;
using Enemy.Launcher;

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
            _steps[2] = new EndStep(requiredRef);
            _steps[1] = new FireStep(requiredRef, _steps[2]);
            _steps[0] = new HoldStep(requiredRef, _steps[1]);
            // 構えと攻撃を繰り返すので、コンストラクタではなく、メソッドから遷移先を追加。
            _steps[2].AddNext(_steps[0]);

            _currentStep = _steps[0];
        }

        protected override void OnEnter()
        {
            Always();
        }

        protected override void OnExit() 
        {
            Always();
        }

        protected override void OnStay()
        {
            Always();

            if (ExitIfDeadOrTimeOver()) return;

            _currentStep = _currentStep.Update();
        }

        public override void Dispose()
        {
            foreach (EnemyActionStep s in _steps) s.Dispose();
        }

        private void Always()
        {
            PlayDamageSE();
            Hovering();
            Move();
            LeftRightMoveAnimation();
            ForwardBackMoveAnimation();
        }
    }
}

namespace Enemy.Launcher
{
    /// <summary>
    /// ロケットランチャー構え。
    /// </summary>
    public class HoldStep : EnemyAttackActionStep
    {
        // UpperBodyのWeight。
        private float _weight;

        public HoldStep(RequiredRef requiredRef, params EnemyActionStep[] next) : base(requiredRef, next)
        {
            // アイドルのアニメーション再生をトリガーする。
            {
                string state = Const.Launcher.Idle;
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
            // ロケットランチャーを構える。
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
    /// ロケットランチャー発射。
    /// </summary>
    public class FireStep : EnemyAttackActionStep
    {
        // アニメーションの再生をトリガーと同時に再生されるとは限らないので、一応フラグ。
        private bool _isAnimationEnter;

        public FireStep(RequiredRef requiredRef, params EnemyActionStep[] next) : base(requiredRef, next)
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
    public class EndStep : EnemyAttackActionStep
    {
        public EndStep(RequiredRef _, params EnemyActionStep[] next) : base(_, next)
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