using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss.BT
{
    public class MoveToPointP : Enemy.Control.BT.Node
    {
        private ActionPlan.Move _movePlan;
        private ActionPlan.Warp _warpPlan;
        private Transform _transform;
        private BossParams _params;
        private BlackBoard _blackBoard;

        public MoveToPointP(Transform transform, BossParams bossParams, BlackBoard blackBoard)
        {
            _movePlan = new ActionPlan.Move(Choice.Chase);
            _warpPlan = new ActionPlan.Warp(Choice.Chase);
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
            // このフレームの移動量が点Pを超えないかチェック
            bool isExceed;
            {
                Vector3 dir = _blackBoard.TransformToPointPDirection;
                float spd = _params.Battle.MoveSpeed;
                float dt = _blackBoard.PausableDeltaTime;
                float toP = _blackBoard.TransformToPointPDistance;

                isExceed = (dir * spd).magnitude * dt >= toP;
            }

            // 移動量が点Pまでの距離を超えてしまう場合は、点Pの位置に移動。
            if (isExceed)
            {
                _warpPlan.Position = _blackBoard.PointP;
                _blackBoard.WarpPlans.Enqueue(_warpPlan);
            }
            else
            {
                _movePlan.Direction = _blackBoard.TransformToPointPDirection;
                _movePlan.Speed = _params.Battle.MoveSpeed;
                _blackBoard.MovePlans.Enqueue(_movePlan);
            }

            return State.Success;
        }
    }
}