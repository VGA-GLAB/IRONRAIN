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
            // イベントのトリガーになるような行動を調べる。
            foreach(ActionPlan plan in _blackBoard.ActionPlans)
            {
                // プレイヤーの左腕破壊をトリガーに、QTEイベントのステートへ遷移。
                if (plan.Choice == Choice.BreakLeftArm) { TryChangeState(stateTable[StateKey.Idle]); return; }

                // 攻撃中だろうが移動中だろうがファンネル展開を実行。
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

            // 攻撃
            foreach (ActionPlan plan in _blackBoard.ActionPlans)
            {
                if (plan.Choice == Choice.BladeAttack)
                {
                    //
                    _blackBoard.LastMeleeAttackTime = Time.time;
                }
                else if (plan.Choice == Choice.RifleFire)
                {
                    //
                    _blackBoard.LastRangeAttackTime = Time.time;
                }
            }
        }

        public override void Dispose()
        {
        }
    }
}