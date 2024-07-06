using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.BT
{
    /// <summary>
    /// 生成位置にワープ。
    /// </summary>
    public class OrderedSpawn : Node
    {
        private ActionPlan.Warp _plan;
        private BlackBoard _blackBoard;

        public OrderedSpawn(Choice choice, BlackBoard blackBoard)
        {
            _plan = new ActionPlan.Warp(choice);
            _blackBoard = blackBoard;
        }

        protected override void OnBreak()
        {
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override State Stay()
        {
            // ここで1度だけを判定すると、Action側のステート遷移を厳密にしないとバグる恐れがある。
            // 優先度を最高にし、他の行動と区別することでAction側のタイミングを縛らないようにする。
            // 生成の命令をされてから毎フレーム生成位置にワープしようとするので、そこそこ不自然。
            if (_blackBoard.OrderedSpawnPoint != null)
            {
                _plan.Position = (Vector3)_blackBoard.OrderedSpawnPoint;
                _plan.Priority = Priority.Critical;
                
                _blackBoard.WarpPlans.Enqueue(_plan);
            }

            return State.Success;
        }
    }
}
