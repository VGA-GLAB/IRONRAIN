using UnityEngine;
using Enemy.Extensions;

namespace Enemy.Boss.FSM
{
    /// <summary>
    /// QTEの各ステップをIDで管理する。
    /// </summary>
    public abstract class QteEventStep
    {
        private bool _isEnter = true;

        public abstract string ID { get; }

        protected abstract void Enter();
        protected abstract QteEventStep Stay();

        /// <summary>
        /// 最初の1回はEnterが呼ばれ、以降はStayが呼ばれる。
        /// </summary>
        public QteEventStep Update()
        {
            if (_isEnter)
            {
                _isEnter = false;
                Enter();
                return this;
            }
            else
            {
                return Stay();
            }
        }
    }

    /// <summary>
    /// 左腕破壊~2回目のQTEまでの一連のイベントのステート。
    /// </summary>
    public class QteEventState : State
    {
        private BlackBoard _blackBoard;
        private AgentScript _agentScript;

        private QteEventStep _moveToPlayerFront;
        private QteEventStep _breakLeftArm;
        private QteEventStep _firstCombat;
        private QteEventStep _firstCombatknockBack;
        private QteEventStep _secondCombat;
        private QteEventStep _secondCombatknockBack;
        private QteEventStep _penetrate;
        private QteEventStep _complete;

        private QteEventStep _currentStep;

        public QteEventState(StateRequiredRef requiredRef) : base(requiredRef.States)
        {
            _blackBoard = requiredRef.BlackBoard;
            _agentScript = requiredRef.AgentScript;

            _complete = new CompleteStep(requiredRef);
            _penetrate = new PenetrateStep(requiredRef, _complete);
            _secondCombatknockBack = new knockBackStep(requiredRef, _penetrate);
            _secondCombat = new SecondCombatStep(requiredRef, _secondCombatknockBack);
            _firstCombatknockBack = new knockBackStep(requiredRef, _secondCombat);
            _firstCombat = new FirstCombatStep(requiredRef, _firstCombatknockBack);
            _breakLeftArm = new BreakLeftArmStep(requiredRef, _firstCombat);
            _moveToPlayerFront = new MoveToPlayerFrontStep(requiredRef, _breakLeftArm);

            _currentStep = _moveToPlayerFront;
        }

        protected override void Enter()
        {
            _blackBoard.CurrentState = StateKey.QteEvent;
        }

        protected override void Exit()
        {
            if (_agentScript != null) _agentScript.EnemyDestory();
        }

        protected override void Stay()
        {
            _currentStep = _currentStep.Update();
        }
    }

    /// <summary>
    /// 刀を振り上げた状態でプレイヤーの正面に移動
    /// </summary>
    public class MoveToPlayerFrontStep : QteEventStep
    {
        private BossParams _params;
        private BlackBoard _blackBoard;
        private Body _body;
        private BodyAnimation _animation;
        private QteEventStep _breakLeftArm;

        public MoveToPlayerFrontStep(StateRequiredRef requiredRef, QteEventStep breakLeftArm)
        {
            _params = requiredRef.BossParams;
            _blackBoard = requiredRef.BlackBoard;
            _body = requiredRef.Body;
            _animation = requiredRef.BodyAnimation;
            _breakLeftArm = breakLeftArm;
        }

        public override string ID => nameof(MoveToPlayerFrontStep);

        protected override void Enter()
        {
            // 刀を構えるアニメーションはMoveからトリガーで遷移するよう設定されているが、
            // Attackの状態の時にQTEイベント開始を呼ばれる可能性があるので、ステートを指定で再生。
            _animation.Play(BodyAnimationConst.Boss.QteSwordSet, BodyAnimationConst.Layer.BaseLayer);
        }

        protected override QteEventStep Stay()
        {
            Vector3 v = VectorExtensions.Homing(
                _blackBoard.Area.Point,
                _blackBoard.PlayerArea.Point,
                _blackBoard.PlayerDirection,
                _params.Other.ApproachHomingPower
                );
            Vector3 velo = v * _params.Qte.ToPlayerFrontMoveSpeed * _blackBoard.PausableDeltaTime;

            // 一定の距離以下になったらフラグを立てる。
            // 単純に直線距離で判定しているのでプレイヤーの後方から近づいても条件を満たしてしまう。
            if (_blackBoard.PlayerSqrDistance < _params.Qte.SocialSqrDistance)
            {
                _blackBoard.IsStandingOnQtePosition = true;
            }
            else
            {
                _body.Warp(_blackBoard.Area.Point + velo);
            }

            // 回転
            Vector3 dir = _blackBoard.PlayerDirection;
            dir.y = 0;
            _body.Forward(dir);

            // 左腕破壊の位置にいるフラグが立っているかつ、左腕破壊の命令がされている場合。
            if (_blackBoard.IsStandingOnQtePosition && _blackBoard.IsBreakLeftArm) return _breakLeftArm;
            else return this;
        }
    }

    /// <summary>
    /// 左腕破壊の演出、プレイヤーの入力で鍔迫り合いに遷移。
    /// </summary>
    public class BreakLeftArmStep : QteEventStep
    {
        private BlackBoard _blackBoard;
        private BodyAnimation _animation;
        private QteEventStep _firstCombat;

        public BreakLeftArmStep(StateRequiredRef requiredRef, QteEventStep firstCombat)
        {
            _blackBoard = requiredRef.BlackBoard;
            _animation = requiredRef.BodyAnimation;
            _firstCombat = firstCombat;
        }

        public override string ID => nameof(BreakLeftArmStep);

        protected override void Enter()
        {
            // プレイヤーの入力があった場合は、刀を振り下ろす。
            _animation.SetTrigger(BodyAnimationConst.Param.QteBladeAttackTrigger01);
        }

        protected override QteEventStep Stay()
        {
            // 左腕破壊 -> 鍔迫り合い1回目 に遷移する命令がされた場合。
            if (_blackBoard.IsQteCombatReady) return _firstCombat;
            else return this;
        }
    }

    /// <summary>
    /// 鍔迫り合い1回目、刀を構え直してプレイヤーの入力で刀を振り下ろす。
    /// </summary>
    public class FirstCombatStep : QteEventStep
    {
        private BlackBoard _blackBoard;
        private BodyAnimation _animation;
        private QteEventStep _firstCombatDelay;

        public FirstCombatStep(StateRequiredRef requiredRef, QteEventStep firstCombatDelay)
        {
            _blackBoard = requiredRef.BlackBoard;
            _animation = requiredRef.BodyAnimation;
            _firstCombatDelay = firstCombatDelay;
        }

        public override string ID => nameof(FirstCombatStep);

        protected override void Enter()
        {
            // 振り下ろした刀を構え直す。
            _animation.SetTrigger(BodyAnimationConst.Param.QteBladeAttackClearTrigger01);
        }

        protected override QteEventStep Stay()
        {
            if (_blackBoard.IsFirstCombatInputed)
            {
                // 鍔迫り合い1回目、刀を振り下ろす。
                _animation.SetTrigger(BodyAnimationConst.Param.QteBladeAttackTrigger02);
                return _firstCombatDelay;
            }
            else
            {
                return this;
            }
        }
    }

    /// <summary>
    /// 刀を振り下ろす -> 弾かれて吹き飛ばされる までがセット。
    /// ディレイを入れて弾かれるタイミングで移動させる。
    /// </summary>
    public class knockBackStep : QteEventStep
    {
        private BossParams _params;
        private BlackBoard _blackBoard;
        private Body _body;
        private QteEventStep _secondCombat;

        private Vector3 _force;
        private float _timer;

        public knockBackStep(StateRequiredRef requiredRef, QteEventStep secondCombat)
        {
            _params = requiredRef.BossParams;
            _blackBoard = requiredRef.BlackBoard;
            _body = requiredRef.Body;
            _secondCombat = secondCombat;
        }

        public override string ID => nameof(knockBackStep);

        protected override void Enter()
        {
            // 振り下ろすアニメーションの長さぶんだけディレイ。
            const float SwingDownDelay = 0.5f;

            _timer = SwingDownDelay;
            _force = -_blackBoard.Forward.normalized * _params.Qte.KnockBackPower;
        }

        protected override QteEventStep Stay()
        {
            const float Friction = 0.98f;
            const float StopThreshold = 1.0f;

            // 刀を振り下ろしている最中は吹き飛ばない。
            _timer -= _blackBoard.PausableDeltaTime;
            if (_timer > 0) return this;
            
            Vector3 v = _force * _blackBoard.PausableDeltaTime;
            _body.Move(v);

            _force *= Friction;

            // ある程度吹き飛ばされて、吹き飛ばし力が弱ったら鍔迫り合い2回目に遷移。
            if (_force.sqrMagnitude < StopThreshold) return _secondCombat;
            else return this;
        }
    }

    /// <summary>
    /// 鍔迫り合い2回目、吹き飛ばされた状態から遷移してくる。
    /// 刀を構え直して突撃、プレイヤーの入力で振り下ろす。
    /// </summary>
    public class SecondCombatStep : QteEventStep
    {
        private BossParams _params;
        private BlackBoard _blackBoard;
        private Body _body;
        private BodyAnimation _animation;
        private QteEventStep _penetrate;

        private Vector3 _force;

        public SecondCombatStep(StateRequiredRef requiredRef, QteEventStep penetrate)
        {
            _params = requiredRef.BossParams;
            _blackBoard = requiredRef.BlackBoard;
            _body = requiredRef.Body;
            _animation = requiredRef.BodyAnimation;
            _penetrate = penetrate;
        }

        public override string ID => nameof(SecondCombatStep);

        protected override void Enter()
        {
            // 振り下ろした刀を構え直す。
            // 1回目の鍔迫り合いのアニメーションを使いまわすのでトリガーではなくPlayで再生するしかない。
            _animation.Play(BodyAnimationConst.Boss.QteSowrdRepel_1, BodyAnimationConst.Layer.UpperBody);

            _force = _blackBoard.PlayerDirection * _params.Qte.ChargeSpeed;
        }

        protected override QteEventStep Stay()
        {
            const float Friction = 0.98f;
            const float Min = 2.0f;

            // プレイヤーとの距離を詰める。
            if(_blackBoard.PlayerSqrDistance > _params.Qte.SocialSqrDistance)
            {
                Vector3 v = _force * _blackBoard.PausableDeltaTime;
                _body.Move(v);

                // 距離を詰め切らないうちに突撃力が尽きないよう、最低値が必要。
                if (_force.sqrMagnitude > Min) _force *= Friction;
            }

            // 鍔迫り合い2回目、刀を振り下ろす。
            // 現状、距離を詰め終わったかの判定をしていないのでシーケンス側で配慮が必要。
            if (_blackBoard.IsSecondCombatInputed)
            {
                _animation.SetTrigger(BodyAnimationConst.Param.QteBladeAttackTrigger02);
                _animation.SetTrigger(BodyAnimationConst.Param.QteBladeAttackClearTrigger02);
                return _penetrate;
            }
            else
            {
                return this;
            }
        }
    }

    /// <summary>
    /// パイルバンカーで貫かれる。
    /// </summary>
    public class PenetrateStep : QteEventStep
    {
        private BossParams _params;
        private BlackBoard _blackBoard;
        private Body _body;
        private BodyAnimation _animation;
        private Effector _effector;
        private QteEventStep _complete;

        private Vector3 _force;

        public PenetrateStep(StateRequiredRef requiredRef, QteEventStep complete)
        {
            _params = requiredRef.BossParams;
            _blackBoard = requiredRef.BlackBoard;
            _body = requiredRef.Body;
            _animation = requiredRef.BodyAnimation;
            _effector = requiredRef.Effector;
            _complete = complete;
        }

        public override string ID => nameof(PenetrateStep);

        protected override void Enter()
        {
            _force = _blackBoard.PlayerDirection * _params.Qte.ChargeSpeed;
        }

        protected override QteEventStep Stay()
        {
            const float Friction = 0.98f;
            const float Min = 2.0f;

            // プレイヤーとの距離を詰める。
            if (_blackBoard.PlayerSqrDistance > _params.Qte.SocialSqrDistance)
            {
                Vector3 v = _force * _blackBoard.PausableDeltaTime;
                _body.Move(v);

                // 距離を詰め切らないうちに突撃力が尽きないよう、最低値が必要。
                if (_force.sqrMagnitude > Min) _force *= Friction;
            }

            // QTE2回目、プレイヤーに殴られて死亡。
            // 現状、距離を詰め終わったかの判定をしていないのでシーケンス側で配慮が必要。
            if (_blackBoard.IsPenetrateInputed)
            {
                _animation.SetTrigger(BodyAnimationConst.Param.FinishTrigger);
                _effector.PlayDestroyedEffect();

                AudioWrapper.PlaySE("SE_Kill");

                return _complete;
            }
            else
            {
                return this;
            }
        }
    }

    /// <summary>
    /// QTEイベントの全操作が完了。
    /// </summary>
    public class CompleteStep : QteEventStep
    {
        public CompleteStep(StateRequiredRef _)
        {
        }

        public override string ID => nameof(CompleteStep);

        protected override void Enter()
        {
        }

        protected override QteEventStep Stay()
        {
            return this;
        }
    }
}