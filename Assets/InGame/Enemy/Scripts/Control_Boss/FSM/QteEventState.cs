using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Control.FSM;

namespace Enemy.Control.Boss.FSM
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

        public QteEventState(BlackBoard blackBoard, BodyAnimation animation, Effector effector, AgentScript agentScript)
        {
            _blackBoard = blackBoard;
            _agentScript = agentScript;

            _breakLeftArmStep = new BreakLeftArmStep(animation);
            _firstQteStep = new FirstQteStep(animation);
            _secondQteStep = new SecondQteStep(animation, effector);
        }

        public override StateKey Key => StateKey.QteEvent;

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
            if (_agentScript != null) _agentScript.EnemyDestory();
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            while(_blackBoard.ActionPlans.TryDequeue(out ActionPlan plan))
            {
                if (plan.Choice == Choice.BreakLeftArm)
                {
                    _breakLeftArmStep.Update();
                }
                else if (plan.Choice == Choice.FirstQte)
                {
                    _firstQteStep.Update();
                }
                else if (plan.Choice == Choice.SecondQte)
                {
                    _secondQteStep.Update();
                }
                else if (plan.Choice == Choice.Broken)
                {
                    TryChangeState(stateTable[StateKey.Idle]);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 左腕破壊
    /// </summary>
    public class BreakLeftArmStep : QteEventStep
    {
        private BodyAnimation _animation;

        public BreakLeftArmStep(BodyAnimation animation)
        {
            _animation = animation;
        }

        protected override void Enter()
        {
            // 刀を構えるアニメーションはMoveからトリガーで遷移するよう設定されているが、
            // Attackの状態の時にQTEイベント開始を呼ばれる可能性があるので、ステートを指定で再生。
            _animation.Play(BodyAnimation.StateName.Boss.QteSwordSet);

            // プレイヤーの入力があった場合は、刀を振り下ろす。
            // 現状、構え->攻撃の間に何か入力を挟む仕様が無いのでそのまま再生する。
            _animation.SetTrigger(BodyAnimation.ParamName.QteBladeAttackTrigger01);

            // 振り下ろした刀を構え直す。
            // これも特に仕様が無いのでそのまま再生する。
            _animation.SetTrigger(BodyAnimation.ParamName.QteBladeAttackClearTrigger01);
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

        public FirstQteStep(BodyAnimation animation)
        {
            _animation = animation;
        }

        protected override void Enter()
        {
            // QTE1回目、刀を振り下ろす。
            _animation.SetTrigger(BodyAnimation.ParamName.QteBladeAttackTrigger02);

            // 振り下ろした刀を構え直す。
            // これも特に仕様が無いのでそのまま再生する。
            _animation.SetTrigger(BodyAnimation.ParamName.QteBladeAttackClearTrigger02);
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

        public SecondQteStep(BodyAnimation animation, Effector effector)
        {
            _animation = animation;
            _effector = effector;
        }

        protected override void Enter()
        {
            // QTE2回目、プレイヤーに殴られて死亡。
            _animation.SetTrigger(BodyAnimation.ParamName.FinishTrigger);
            _effector.PlayDestroyedEffect();
        }

        protected override void Stay()
        {            
        }
    }
}