using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Control.FSM;

namespace Enemy.Control.Boss.FSM
{
    /// <summary>
    /// 登場後、戦闘するステート。
    /// </summary>
    public class BattleState : State
    {
        private enum AnimationGroup
        {
            Other,       // 初期状態
            Idle,        // Idle
            Hold,        // HoldStart~HoldLoop
            Fire,        // FireLoop
            Blade,       // BladeStart~BladeLoop
            BladeAttack, // BladeAttack
        }

        private BlackBoard _blackBoard;
        private Body _body;
        private BodyAnimation _animation;
        private IReadOnlyCollection<FunnelController> _funnels;

        // 現在のアニメーションのステートによって処理を分岐するために使用する。
        private AnimationGroup _currentAnimGroup;

        public BattleState(BlackBoard blackBoard, Body body, BodyAnimation bodyAnimation, 
            IReadOnlyCollection<FunnelController> funnels)
        {
            _blackBoard = blackBoard;
            _body = body;
            _animation = bodyAnimation;
            _funnels = funnels;

            // アニメーションのステートの遷移をトリガーする。
            Register(BodyAnimation.StateName.Boss.Idle, AnimationGroup.Idle);
            Register(BodyAnimation.StateName.Boss.HoldStart, AnimationGroup.Hold);
            Register(BodyAnimation.StateName.Boss.FireLoop, AnimationGroup.Fire);
            Register(BodyAnimation.StateName.Boss.BladeStart, AnimationGroup.Blade);
            Register(BodyAnimation.StateName.Boss.BladeAttack, AnimationGroup.BladeAttack);

            // stateNameのアニメーションのステートに遷移してきたタイミング(Enter)のみトリガーしている。
            // このメソッドで登録していないアニメーションのステートに遷移した場合、
            // _currentAnimGroupの値が元のままになるので注意。
            void Register(string stateName, AnimationGroup animGroup)
            {
                _animation.RegisterStateEnterCallback(Key, stateName, () => _currentAnimGroup = animGroup);
            }
        }

        public override StateKey Key => StateKey.Battle;

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            // イベントのトリガーになるような行動を調べる。
            foreach(ActionPlan plan in _blackBoard.ActionPlans)
            {
                // プレイヤーの左腕破壊をトリガーに、QTEイベントのステートへ遷移。
                if (plan.Choice == Choice.BreakLeftArm) { TryChangeState(stateTable[StateKey.Idle]); return; }

                // 攻撃中だろうが移動中だろうがファンネル展開を実行。
                if (plan.Choice == Choice.FunnelExpand && _funnels != null)
                {
                    foreach (FunnelController f in _funnels) f.Expand();
                }
            }

            // 移動を上書きする恐れがあるので、先に座標を直接書き換える。
            while (_blackBoard.WarpPlans.TryDequeue(out ActionPlan.Warp plan))
            {
                if (plan.Choice == Choice.Chase) _body.Warp(plan.Position);
            }
            // 移動
            while (_blackBoard.MovePlans.TryDequeue(out ActionPlan.Move plan))
            {
                if (plan.Choice == Choice.Chase)
                {
                    _body.Move(plan.Direction * plan.Speed * _blackBoard.PausableDeltaTime);
                }
            }
            // 回転
            while (_blackBoard.LookPlans.TryDequeue(out ActionPlan.Look plan))
            {
                if (plan.Choice == Choice.Chase) _body.Forward(plan.Forward);
            }

            // どのアニメーションが再生されているかによって処理を分ける。
            if (_currentAnimGroup == AnimationGroup.Idle) StayIdle();
            else if (_currentAnimGroup == AnimationGroup.Hold) StayHold();
            else if (_currentAnimGroup == AnimationGroup.Fire) StayFire();
            else if (_currentAnimGroup == AnimationGroup.Blade) StayBlade();
            else if (_currentAnimGroup == AnimationGroup.BladeAttack) StayBladeAttack();
            else StayOther();
        }

        public override void Dispose()
        {
        }

        // アニメーションがアイドル状態
        private void StayIdle()
        {
            // 攻撃可能なタイミングになった場合、攻撃するまで毎フレーム書き込まれる。
            // Brain側はアニメーションの状態を把握していないので、ここで調整する必要がある。
            foreach (ActionPlan plan in _blackBoard.ActionPlans)
            {
                // 攻撃のアニメーション再生をトリガー。
                if (plan.Choice == Choice.BladeAttack)
                {
                    _animation.SetTrigger(BodyAnimation.ParamName.TempBladeAttackSetTrigger);
                    _animation.ResetTrigger(BodyAnimation.ParamName.AttackSetTrigger);
                    break;
                }
                else if (plan.Choice == Choice.RifleFire)
                {
                    _animation.SetTrigger(BodyAnimation.ParamName.AttackSetTrigger);
                    _animation.ResetTrigger(BodyAnimation.ParamName.TempBladeAttackSetTrigger);
                    break;
                }
            }
        }

        // アニメーションが銃構え状態
        private void StayHold()
        {
            // 現状、特にプランナーから指示が無いので構え->発射を瞬時に行う。
            _animation.SetTrigger(BodyAnimation.ParamName.AttackTrigger);
        }

        // アニメーションが銃攻撃状態
        private void StayFire()
        {
            // 射撃のアニメーションのステートが繰り返されるようになっているため、
            // 手動で射撃終了をトリガーしないと近接攻撃出来ない。
            foreach (ActionPlan plan in _blackBoard.ActionPlans)
            {
                if (plan.Choice == Choice.BladeAttack)
                {
                    _animation.SetTrigger(BodyAnimation.ParamName.AttackEndTrigger);
                }
            }
        }

        // アニメーションが刀構え状態
        private void StayBlade()
        {
            // 現状、特にプランナーから指示が無いので構え->攻撃を瞬時に行う。
            _animation.SetTrigger(BodyAnimation.ParamName.TempBladeAttackTrigger);
        }

        // アニメーションが刀攻撃状態
        private void StayBladeAttack()
        {
            //
        }

        // アニメーションがそれ以外状態
        private void StayOther()
        {
            //
        }
    }
}