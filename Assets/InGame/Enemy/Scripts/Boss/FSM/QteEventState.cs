﻿namespace Enemy.Boss.FSM
{
    /// <summary>
    /// QTEの各ステップを管理する。
    /// </summary>
    public abstract class QteEventStep
    {
        private bool _isEnter = true;

        protected abstract void Enter();
        protected abstract void Stay();

        /// <summary>
        /// 最初の1回はEnterが呼ばれ、以降はStayが呼ばれる。
        /// </summary>
        public void Update()
        {
            if (_isEnter)
            {
                _isEnter = false;
                Enter();
            }
            else
            {
                Stay();
            }
        }
    }

    /// <summary>
    /// 左腕破壊~2回目のQTEまでの一連のイベントのステート。
    /// </summary>
    public class QteEventState : State
    {
        public enum Step
        {
            Other,
            BreakLeftArm,
            FirstQte,
            SecondQte,
        }

        private BlackBoard _blackBoard;
        private AgentScript _agentScript;

        private BreakLeftArmStep _breakLeftArmStep;
        private FirstQteStep _firstQteStep;
        private SecondQteStep _secondQteStep;

        public QteEventState(StateRequiredRef requiredRef) : base(requiredRef.States)
        {
            _blackBoard = requiredRef.BlackBoard;
            _agentScript = requiredRef.AgentScript;

            _breakLeftArmStep = new BreakLeftArmStep(requiredRef);
            _firstQteStep = new FirstQteStep(requiredRef);
            _secondQteStep = new SecondQteStep(requiredRef);
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
            if (_agentScript != null) _agentScript.EnemyDestory();
        }

        protected override void Stay()
        {
            if (_blackBoard.OrderdQteEventStep == Step.BreakLeftArm) _breakLeftArmStep.Update();
            else if (_blackBoard.OrderdQteEventStep == Step.FirstQte) _firstQteStep.Update();
            else if (_blackBoard.OrderdQteEventStep == Step.SecondQte) _secondQteStep.Update();
        }
    }

    /// <summary>
    /// 左腕破壊
    /// </summary>
    public class BreakLeftArmStep : QteEventStep
    {
        private BodyAnimation _animation;

        public BreakLeftArmStep(StateRequiredRef requiredRef)
        {
            _animation = requiredRef.BodyAnimation;
        }

        protected override void Enter()
        {
            // 刀を構えるアニメーションはMoveからトリガーで遷移するよう設定されているが、
            // Attackの状態の時にQTEイベント開始を呼ばれる可能性があるので、ステートを指定で再生。
            _animation.Play(BodyAnimationConst.Boss.QteSwordSet, BodyAnimationConst.Layer.BaseLayer);

            // プレイヤーの入力があった場合は、刀を振り下ろす。
            // 現状、構え->攻撃の間に何か入力を挟む仕様が無いのでそのまま再生する。
            _animation.SetTrigger(BodyAnimationConst.Param.QteBladeAttackTrigger01);

            // 振り下ろした刀を構え直す。
            // これも特に仕様が無いのでそのまま再生する。
            _animation.SetTrigger(BodyAnimationConst.Param.QteBladeAttackClearTrigger01);
        }

        protected override void Stay()
        {
        }
    }

    /// <summary>
    /// 1回目のQTE
    /// </summary>
    public class FirstQteStep : QteEventStep
    {
        private BodyAnimation _animation;

        public FirstQteStep(StateRequiredRef requiredRef)
        {
            _animation = requiredRef.BodyAnimation;
        }

        protected override void Enter()
        {
            // QTE1回目、刀を振り下ろす。
            _animation.SetTrigger(BodyAnimationConst.Param.QteBladeAttackTrigger02);

            // 振り下ろした刀を構え直す。
            // これも特に仕様が無いのでそのまま再生する。
            _animation.SetTrigger(BodyAnimationConst.Param.QteBladeAttackClearTrigger02);
        }

        protected override void Stay()
        {

        }
    }

    /// <summary>
    /// 2回目のQTE
    /// </summary>
    public class SecondQteStep : QteEventStep
    {
        private BodyAnimation _animation;
        private Effector _effector;

        public SecondQteStep(StateRequiredRef requiredRef)
        {
            _animation = requiredRef.BodyAnimation;
            _effector = requiredRef.Effector;
        }

        protected override void Enter()
        {
            // QTE2回目、プレイヤーに殴られて死亡。
            _animation.SetTrigger(BodyAnimationConst.Param.FinishTrigger);
            _effector.PlayDestroyedEffect();
        }

        protected override void Stay()
        {            
        }
    }
}