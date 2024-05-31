using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
{
    public class BlackBoard
    {
        public BlackBoard()
        {
            WarpOptions = new Queue<WarpPlan>();
            MovementOptions = new Queue<MovementPlan>();
        }

        // ブレインが書き込む。
        // Updateで書き込まれ、LateUpdateで消えるのでフレームを跨がない。
        public Queue<WarpPlan> WarpOptions { get; private set; }
        public Queue<MovementPlan> MovementOptions { get; private set; }

        // センサーが書き込む。
        // Startでインスタンスが確保、Updateで値が更新され、Disableでnull。
        public CircleArea Area { get; set; }
        public CircleArea PlayerArea { get; set; }
        public Vector3 PointP { get; set; }

        public void AddWarpOption(Choice choice, Vector3 position)
        {
            WarpOptions.Enqueue(new WarpPlan { Choice = choice, Position = position });
        }

        public void AddMovementOption(Choice choice, Vector3 direction, float speed)
        {
            MovementOptions.Enqueue(new MovementPlan { Choice = choice, Direction = direction, Speed = speed });
        }
    }
}
