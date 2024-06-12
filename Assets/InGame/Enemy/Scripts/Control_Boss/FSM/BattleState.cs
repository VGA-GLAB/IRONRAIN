using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss.FSM
{
    /// <summary>
    /// 登場後、戦闘するステート。
    /// </summary>
    public class BattleState : State
    {
        private enum AnimationGroup
        {
            Other,  // 初期状態
        }

        private BlackBoard _blackBoard;
        private Body _body;
        private IReadOnlyCollection<FunnelController> _funnels;

        // 現在のアニメーションのステートによって処理を分岐するために使用する。
        private AnimationGroup _currentAnimGroup;

        public BattleState(BlackBoard blackBoard, Body body, IReadOnlyCollection<FunnelController> funnels)
        {
            _blackBoard = blackBoard;
            _body = body;
            _funnels = funnels;
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
            // 行動を調べる。
            while (_blackBoard.ActionPlans.TryDequeue(out ActionPlan plan))
            {
                // ファンネル展開
                if (plan.Choice == Choice.FunnelExpand && _funnels != null)
                {
                    foreach (FunnelController f in _funnels) f.OpenAnimation();
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
        }

        public override void Dispose()
        {
        }
    }
}