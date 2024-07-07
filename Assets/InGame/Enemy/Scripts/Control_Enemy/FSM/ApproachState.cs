using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// 画面に表示され、スロット位置まで接近するステート。
    /// </summary>
    public class ApproachState : State
    {
        private BlackBoard _blackBoard;
        private Body _body;
        private BodyAnimation _animation;

        public ApproachState(BlackBoard blackBoard, Body body, BodyAnimation animation)
        {
            _blackBoard = blackBoard;
            _body = body;
            _animation = animation;
        }

        public override StateKey Key => StateKey.Approach;

        protected override void Enter()
        {
            // 生成位置へワープする命令
            foreach (ActionPlan.Warp plan in _blackBoard.WarpPlans)
            {
                if (plan.Priority == Priority.Critical)
                {
                    _body.Warp(plan.Position);
                }
            }
        }

        protected override void Exit()
        {
            // 接近アニメーション終了をトリガー。
            _animation.SetTrigger(BodyAnimation.ParamName.ApproachEndTrigger);
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            // ダメージを受けた場合に音を再生。
            if (_blackBoard.DamageSource != "")
            {
                AudioWrapper.PlaySE("SE_Missile_Hit");
            }

            // 死んだかチェック。
            bool isDead = false;
            foreach (ActionPlan plan in _blackBoard.ActionPlans)
            {
                if (plan.Choice == Choice.Broken) isDead = true;
            }

            // 死亡した場合はアイドル状態を経由して死亡ステートに遷移する。
            if (isDead) { TryChangeState(stateTable[StateKey.Idle]); return; }

            // スロット位置へ接近中かがこのステートの終了条件。
            // 接近するための移動や回転を行わないフレームがあれば条件を満たして遷移。
            bool isApproaching = false;

            // 座標を直接書き換える。
            // deltaTimeぶんの移動を上書きする恐れがあるので移動より先。
            while (_blackBoard.WarpPlans.TryDequeue(out ActionPlan.Warp plan))
            {
                if (plan.Choice != Choice.Approach) continue;

                _body.Warp(plan.Position);
                isApproaching = true;
            }

            // 移動
            while (_blackBoard.MovePlans.TryDequeue(out ActionPlan.Move plan))
            {
                if (plan.Choice != Choice.Approach) continue;

                _body.Move(plan.Direction * plan.Speed * _blackBoard.PausableDeltaTime);
                isApproaching = true;
            }

            // 回転
            while (_blackBoard.LookPlans.TryDequeue(out ActionPlan.Look plan))
            {
                if (plan.Choice != Choice.Approach) continue;

                _body.Forward(plan.Forward);
                isApproaching = true;
            }

            // 接近アニメーション終了は次フレームでExitが呼ばれたタイミング。
            if (!isApproaching) TryChangeState(stateTable[StateKey.Idle]);
        }
    }
}