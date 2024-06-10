using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
{
    public class BlackBoard
    {
        public BlackBoard()
        {
            WarpPlans = new Queue<ActionPlan.Warp>();
            MovePlans = new Queue<ActionPlan.Move>();
        }

        // ブレインが書き込む。
        // Updateで書き込まれ、LateUpdateで消えるのでフレームを跨がない。
        public Queue<ActionPlan.Warp> WarpPlans { get; private set; }
        public Queue<ActionPlan.Move> MovePlans { get; private set; }

        // センサーが書き込む。
        // Startでインスタンスが確保、Updateで値が更新され、Disableでnull。
        public CircleArea Area { get; set; }
        public CircleArea PlayerArea { get; set; }
        public Vector3 PointP { get; set; }

        public void AddWarpOption(Choice choice, Vector3 position)
        {
            //
        }

        public void AddMovementOption(Choice choice, Vector3 direction, float speed)
        {
            // 
        }
    }
}
