using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// スロット位置まで接近するステート
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
            _animation.Play(Const.AnimationName.ApproachEnter);
        }

        protected override void Exit()
        {
            _animation.Play(Const.AnimationName.Idle);
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            if (IsDead())
            {
                TryChangeState(stateTable[StateKey.Idle]);
            }
            else
            {
                Approach(stateTable);
            }  
        }

        // 死んだかチェック
        private bool IsDead()
        {
            foreach (ActionPlan plan in _blackBoard.ActionOptions)
            {
                if (plan.Choice == Choice.Broken) return true;
            }

            return false;
        }

        // 移動させ、移動完了した場合は遷移
        private void Approach(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            // スロット位置への接近中かの判定がこのステートの終了条件。
            // 接近するための移動や回転を行わないフレームがあれば条件を満たして遷移。
            bool isApproaching = false;

            // 座標を直接書き換える。
            // deltaTimeぶんの移動を上書きする恐れがあるので移動より先。
            while (_blackBoard.WarpOptions.TryDequeue(out WarpPlan plan))
            {
                if (plan.Choice != Choice.Approach) continue;

                _body.Warp(plan.Position);
                isApproaching = true;
            }

            // 移動
            while (_blackBoard.MovementOptions.TryDequeue(out MovementPlan plan))
            {
                if (plan.Choice != Choice.Approach) continue;

                _body.Move(plan.Direction * plan.Speed);
                isApproaching = true;
            }

            // 回転
            while (_blackBoard.ForwardOptions.TryDequeue(out ForwardPlan plan))
            {
                if (plan.Choice != Choice.Approach) continue;

                _body.Forward(plan.Value);
                isApproaching = true;
            }

            if (!isApproaching) TryChangeState(stateTable[StateKey.Idle]);
        }
    }
}