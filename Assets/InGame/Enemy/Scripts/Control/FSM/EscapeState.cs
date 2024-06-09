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
            while (_blackBoard.WarpOptions.TryDequeue(out WarpPlan plan))
            {
                if (plan.Choice == Choice.Escape) _body.Warp(plan.Position);
            }
            // 移動
            while (_blackBoard.MovementOptions.TryDequeue(out MovementPlan plan))
            {
                if (plan.Choice == Choice.Escape) _body.Move(plan.Direction * plan.Speed);
            }
            // 回転
            while (_blackBoard.ForwardOptions.TryDequeue(out ForwardPlan plan))
            {
                if (plan.Choice == Choice.Escape) _body.Forward(plan.Value);
            }
        }
    }
}
