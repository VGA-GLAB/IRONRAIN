using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
{
    public class Brain
    {
        private BlackBoard _blackBoard;

        public Brain(BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
        }

        public void UpdateEvent()
        {
            // 移動のみテスト。本来はビヘイビアツリー？
            Vector3 dir = _blackBoard.PointP - _blackBoard.Area.Point;
            Vector3 warp = _blackBoard.Area.Point + dir * Time.deltaTime * 5;
            _blackBoard.AddWarpOption(Choice.Idle, warp);
        }

        public void LateUpdateEvent()
        {
            _blackBoard.WarpOptions.Clear();
        }
    }
}
