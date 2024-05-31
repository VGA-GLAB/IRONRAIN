using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
{
    public class Brain : LifeCycle
    {
        private BlackBoard _blackBoard;

        public Brain(BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
        }

        public override Result UpdateEvent()
        {
            // 移動のみテスト。本来はビヘイビアツリー？
            Vector3 dir = _blackBoard.PointP - _blackBoard.Area.Point;
            Vector3 warp = _blackBoard.Area.Point + dir * Time.deltaTime * 5;
            _blackBoard.AddWarpOption(Choice.Idle, warp);

            return Result.Running;
        }

        public override Result LateUpdateEvent()
        {
            _blackBoard.WarpOptions.Clear();

            return Result.Running;
        }
    }
}
