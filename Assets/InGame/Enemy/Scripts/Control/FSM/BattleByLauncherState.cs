﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// 移動しつつ攻撃するステート。
    /// 接近後、アイドル状態を経由して遷移してくる。
    /// </summary>
    public class BattleByLauncherState : State
    {
        private enum AnimationGroup
        {
            Other,  // 初期状態
            Idle,   // Idle
            Hold,   // HoldStart~HoldLoop
            Fire,   // Fire
            Reload, // Reload
        }

        private BlackBoard _blackBoard;
        private Body _body;
        private BodyAnimation _animation;

        // 現在のアニメーションのステートによって処理を分岐するために使用する。
        private AnimationGroup _currentAnimGroup;

        public BattleByLauncherState(BlackBoard blackBoard, Body body, BodyAnimation animation)
        {
            _blackBoard = blackBoard;
            _body = body;
            _animation = animation;

            // アニメーションのステートの遷移をトリガーする。
            Register(BodyAnimation.StateName.Launcher.Idle, AnimationGroup.Idle);
            Register(BodyAnimation.StateName.Launcher.HoldStart, AnimationGroup.Hold);
            Register(BodyAnimation.StateName.Launcher.Fire, AnimationGroup.Fire);
            Register(BodyAnimation.StateName.Launcher.Reload, AnimationGroup.Reload);

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
            foreach (ActionPlan plan in _blackBoard.ActionOptions)
            {
                if (plan.Choice == Choice.Broken || plan.Choice == Choice.Escape) isExit = true;
            }

            // 死亡もしくは撤退の場合は、アイドル状態を経由して退場するステートに遷移する。
            if (isExit) { TryChangeState(stateTable[StateKey.Idle]); return; }

            // 左右どちらに移動したかを判定する用途。
            Vector3 before = _body.TransformPosition;

            // 移動を上書きする恐れがあるので、先に座標を直接書き換える。
            while (_blackBoard.WarpOptions.TryDequeue(out WarpPlan plan))
            {
                if (plan.Choice == Choice.Chase) _body.Warp(plan.Position);
            }
            // 移動
            while (_blackBoard.MovementOptions.TryDequeue(out MovementPlan plan))
            {
                if (plan.Choice == Choice.Chase) _body.Move(plan.Direction * plan.Speed);
            }
            // 回転
            while (_blackBoard.ForwardOptions.TryDequeue(out ForwardPlan plan))
            {
                if (plan.Choice == Choice.Chase) _body.Forward(plan.Value);
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
            else if (_currentAnimGroup == AnimationGroup.Hold) StayHold();
            else if (_currentAnimGroup == AnimationGroup.Fire) StayFire();
            else if (_currentAnimGroup == AnimationGroup.Reload) StayReload();
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
            while (_blackBoard.ActionOptions.TryDequeue(out ActionPlan plan))
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

        // アニメーションが武器構え状態
        private void StayHold()
        {
            // 現状、特にプランナーから指示が無いので構え->発射を瞬時に行う。
            _animation.SetTrigger(BodyAnimation.ParamName.AttackTrigger);
        }

        // アニメーションが攻撃状態
        private void StayFire()
        {
            // AnimationがMissingになっており、アニメーションイベントに登録できないのでまだ。
        }

        // アニメーションが武器リロード状態
        private void StayReload()
        {
        }

        // アニメーションがそれ以外状態
        private void StayOther()
        {
        }
    }
}