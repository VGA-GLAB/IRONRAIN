using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss.BT
{
    public class MoveToPointP : Enemy.Control.BT.Node
    {
        private ActionPlan.Move _plan;
        private Transform _transform;
        private BossParams _params;
        private BlackBoard _blackBoard;

        public MoveToPointP(Transform transform, BossParams bossParams, BlackBoard blackBoard)
        {
            _plan = new ActionPlan.Move(Choice.Chase);
            _transform = transform;
            _params = bossParams;
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
            // 向きと速さを設定して移動させる。
            // 極端に近づくとガクガクする処理を直していない。
            _plan.Direction = _blackBoard.TransformToPointPDirection;
            _plan.Speed = _params.Battle.MoveSpeed;
            _blackBoard.MovePlans.Enqueue(_plan);

            return State.Success;
        }
    }
}