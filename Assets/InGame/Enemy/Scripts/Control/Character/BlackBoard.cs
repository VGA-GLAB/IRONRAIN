using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// キャラクターの情報を各層で共有する。
    /// </summary>
    public class BlackBoard
    {
        public BlackBoard()
        {
            ActionOptions = new Queue<ActionPlan>();
            WarpOptions = new Queue<WarpPlan>();
            MovementOptions = new Queue<MovementPlan>();
            ForwardOptions = new Queue<ForwardPlan>();
            FovEnter = new HashSet<Collider>();
            FovStay = new HashSet<Collider>();
            FovExit = new HashSet<Collider>();
        }

        // ビヘイビアツリーの各ノードがキューイング、Action側で制御完了時にクリアされる。
        public Queue<ActionPlan> ActionOptions { get; private set; }
        public Queue<WarpPlan> WarpOptions { get; private set; }
        public Queue<MovementPlan> MovementOptions { get; private set; }
        public Queue<ForwardPlan> ForwardOptions { get; private set; }

        public void AddActionOptions(Choice choice)
        {
            ActionOptions.Enqueue(new ActionPlan { Choice = choice });
        }
        public void AddWarpOption(Choice choice, Vector3 position)
        {
            WarpOptions.Enqueue(new WarpPlan { Choice = choice, Position = position });
        }
        public void AddMovementOption(Choice choice, Vector3 direction, float speed)
        {
            MovementOptions.Enqueue(new MovementPlan { Choice = choice, Direction = direction, Speed = speed });
        }
        public void AddForwardOption(Choice choice, Vector3 forward)
        {
            ForwardOptions.Enqueue(new ForwardPlan { Choice = choice, Value = forward });
        }

        // 視界センサーが書き込む。
        // Updateで書きこまれ、LateUpdateで消える。
        public HashSet<Collider> FovEnter { get; private set; }
        public HashSet<Collider> FovStay { get; private set; }
        public HashSet<Collider> FovExit { get; private set; }

        // 位置関係を調べるセンサーが書き込む。
        // Startでインスタンスが確保、Updateで値が更新され、Disableでnull。
        public CircleArea Area { get; set; }
        public CircleArea PlayerArea { get; set; }
        public CircleArea Slot { get; set; }

        // 位置関係を調べるセンサーが書き込む。
        // Updateで値が更新される。
        public Vector3 AreaToSlotDirection { get; set; }
        public float AreaToSlotSqrDistance { get; set; }
        public Vector3 PlayerPosition { get; set; }
        public Vector3 TransformToPlayerDirection { get; set; }
        public float TransformToPlayerDistance { get; set; }

        // センサー側で次の攻撃タイミングを書き込む。
        // Updateで値が更新される。
        public float NextAttackTime;
        // 攻撃した際にこの値をTimeに書き換えることで、次の攻撃タイミングが更新される。
        public float LastAttackTime;

        // 自身の状態をチェックするセンサーが書き込む。
        // Updateで値が更新される。
        public int Hp { get; set; }
        public bool IsDying { get; set; }
        public float LifeTime { get; set; }

        public bool IsAlive() => Hp > 0;
        public bool IsFine() => !IsDying;
        public bool IsInTime() => LifeTime > 0;
    }
}
