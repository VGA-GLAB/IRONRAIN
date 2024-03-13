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
            WarpOptions = new Queue<DeltaWarp>();
            MovementOptions = new Queue<DeltaMovement>();
            FovEnter = new HashSet<Collider>();
            FovStay = new HashSet<Collider>();
            FovExit = new HashSet<Collider>();
        }

        // ビヘイビアツリーがキューイング、Action側で制御完了時にクリアされる。
        public Queue<DeltaWarp> WarpOptions { get; private set; }
        public Queue<DeltaMovement> MovementOptions { get; private set; }

        public void AddWarpOption(Choice choice, Vector3 position)
        {
            WarpOptions.Enqueue(new DeltaWarp { Choice = choice, Position = position });
        }
        public void AddMovementOption(Choice choice, Vector3 direction, float speed)
        {
            MovementOptions.Enqueue(new DeltaMovement { Choice = choice, Direction = direction, Speed = speed });
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
        public Vector3 TransformToAreaDirection { get; set; } // まだ書き込んでいない。
        public float TransformToAreaDistance { get; set; } // まだ書き込んでいない。

        // 以下3つはまだ書き込んでいない。
        public int Hp { get; set; }
        public bool IsDying { get; set; }
        public float LifeTime { get; set; }

        public bool IsAlive() => Hp > 0;
        public bool IsFine() => !IsDying;
        public bool IsInTime() => LifeTime > 0;
    }
}
