﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// 移動しつつ攻撃するステート。
    /// 接近後、アイドル状態を経由して遷移してくる。
    /// </summary>
    public class BattleByShieldState : State
    {
        private enum AnimationGroup
        {
            Other,  // 初期状態
            Idle,   // Idle
            Shield, // ShieldStart~ShieldLoop
            Attack, // Attack
        }

        private BlackBoard _blackBoard;
        private Body _body;
        private BodyAnimation _animation;

        // 現在のアニメーションのステートによって処理を分岐するために使用する。
        private AnimationGroup _currentAnimGroup;

        public BattleByShieldState(BlackBoard blackBoard, Body body, BodyAnimation animation)
        {
            _blackBoard = blackBoard;
            _body = body;
            _animation = animation;

            // アニメーションのステートの遷移をトリガーする。
            Register(BodyAnimation.StateName.Shield.Idle, AnimationGroup.Idle);
            Register(BodyAnimation.StateName.Shield.ShieldStart, AnimationGroup.Shield);
            Register(BodyAnimation.StateName.Shield.Attack, AnimationGroup.Attack);

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
            // 死亡と撤退どちらの場合でも、武器を下ろすアニメーションをトリガー。
            _animation.SetTrigger(BodyAnimation.ParamName.AttackEndTrigger);
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            // 死亡もしくは撤退をチェック。
            bool isExit = false;
            foreach (ActionPlan plan in _blackBoard.ActionPlans)
            {
                if (plan.Choice == Choice.Broken || plan.Choice == Choice.Escape) isExit = true;
            }

            // 死亡もしくは撤退の場合は、アイドル状態を経由して退場するステートに遷移する。
            if (isExit) { TryChangeState(stateTable[StateKey.Idle]); return; }

            // 左右どちらに移動したかを判定する用途。
            Vector3 before = _body.TransformPosition;

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

            // 構え->攻撃のアニメーションをループする仕様。
            // 今の仕様だと別レイヤーにある左右移動のアニメーションを並行して再生できない。
            {
                // 移動した方向ベクトルでアニメーションを制御。
                // z軸を前方向として、ベクトルのx成分の正負で左右どちらに移動したかを判定する。
                //bool isRightMove = _body.TransformPosition.x - before.x > 0;
                //_animation.SetBool(Const.AnimationParam.IsRightMove, isRightMove);
                //_animation.SetBool(Const.AnimationParam.IsLeftMove, !isRightMove);
            }

            // どのアニメーションが再生されているかによって処理を分ける。
            if (_currentAnimGroup == AnimationGroup.Idle) StayIdle();
            else if (_currentAnimGroup == AnimationGroup.Shield) StayShield();
            else if (_currentAnimGroup == AnimationGroup.Attack) StayAttack();
            else StayOther();
        }

        public override void Dispose()
        {
            // コールバックの登録解除。
            _animation.ReleaseStateCallback(Key);
        }

        // アニメーションがアイドル状態
        private void StayIdle()
        {
            // 攻撃可能なタイミングになった場合、攻撃するまで毎フレーム書き込まれる。
            // Brain側はアニメーションの状態を把握していないので、ここで調整する必要がある。
            while (_blackBoard.ActionPlans.TryDequeue(out ActionPlan plan))
            {
                // 攻撃のアニメーション再生をトリガー。
                // ステートに遷移してきたEnterのタイミングで、武器を構えるアニメーション再生がトリガーされている。
                if (plan.Choice == Choice.Attack)
                {
                    _animation.SetTrigger(BodyAnimation.ParamName.AttackSetTrigger);
                    // 最後に攻撃した時間を更新しているが、本当にここで大丈夫か要チェック。
                    // トリガーから実際に再生されるまでのラグ(HasExitTime設定など)があるかもしれない？
                    _blackBoard.LastAttackTime = Time.time;
                }
            }
        }

        // アニメーションが盾構え状態
        private void StayShield()
        {
            // 現状、特にプランナーから指示が無いので構え->発射を瞬時に行う。
            _animation.SetTrigger(BodyAnimation.ParamName.AttackTrigger);
        }

        // アニメーションが攻撃状態
        private void StayAttack()
        {
            // AnimationがMissingになっており、アニメーションイベントに登録できないのでまだ。
        }

        // アニメーションがそれ以外状態
        private void StayOther()
        {
        }
    }
}