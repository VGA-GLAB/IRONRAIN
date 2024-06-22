using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// 画面外に逃げ出すステート
    /// </summary>
    public class EscapeState : State
    {
        private BlackBoard _blackBoard;
        private Body _body;
        private BodyAnimation _animation;

        public EscapeState(BlackBoard blackBoard, Body body, BodyAnimation animation)
        {
            _blackBoard = blackBoard;
            _body = body;
            _animation = animation;
        }

        public override StateKey Key => StateKey.Escape;

        protected override void Enter()
        {

        }

        protected override void Exit()
        {

        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            // 移動を上書きする恐れがあるので、先に座標を直接書き換える。
            while (_blackBoard.WarpPlans.TryDequeue(out ActionPlan.Warp plan))
            {
                if (plan.Choice == Choice.Escape) _body.Warp(plan.Position);
            }
            // 移動
            while (_blackBoard.MovePlans.TryDequeue(out ActionPlan.Move plan))
            {
                if (plan.Choice == Choice.Escape)
                {
                    _body.Move(plan.Direction * plan.Speed * _blackBoard.PausableDeltaTime);
                }
            }
            // 回転
            while (_blackBoard.LookPlans.TryDequeue(out ActionPlan.Look plan))
            {
                if (plan.Choice == Choice.Escape) _body.Forward(plan.Forward);
            }

            // 画面から消えた場合は後始末の処理を呼んでも問題ない。
            while (_blackBoard.ActionPlans.TryDequeue(out ActionPlan plan))
            {
                if (plan.Choice == Choice.Hide) _blackBoard.IsCleanupReady = true;
            }
        }
    }
}
