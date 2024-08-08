using Enemy.Boss.FSM;
using UnityEngine;

namespace Enemy.Boss
{
    /// <summary>
    /// キャラクターの情報を各層で共有する。
    /// </summary>
    public class BlackBoard : IReadonlyBlackBoard
    {
        public BlackBoard(string name = "")
        {
            ID = System.Guid.NewGuid();
            Name = name;
        }

        // 外部から個体毎の判定が出来る。
        // コンストラクタで生成。
        public System.Guid ID { get; private set; }
        public string Name { get; private set; }

        // 現状ボス戦ではポーズ処理が無いが一応。
        public float PausableDeltaTime => Time.deltaTime;

        // Perceptionクラスが書き込む。
        // Startでインスタンスが確保、Updateで値が更新され、Disableでnull。
        public Area Area { get; set; }
        public Area PlayerArea { get; set; }

        // Perceptionクラスが書き込む。
        // Updateで値が更新される。
        public Vector3 Forward { get; set; }
        public Vector3 PointP { get; set; }
        public Vector3 TransformToPointPDirection { get; set; }
        public float TransformToPointPDistance { get; set; }
        public Vector3 TransformToPlayerDirection { get; set; }
        public float TransformToPlayerSqrDistance { get; set; }
        public bool IsWithinMeleeRange { get; set; }
        public float ElapsedTime { get; set; }
        public bool IsAppearCompleted { get; set; }

        // FireRateクラスが書き込む。
        // Updateで値が更新される。
        public float NextRangeAttackTime { get; set; }
        public float NextMeleeAttackTime { get; set; }

        // HitPointクラスで書き込む。
        // Updateで値が更新される。
        public int Damage { get; set; }
        public string DamageSource { get; set; }

        // OverrideOrderクラスが書き込む。
        // Updateで値が更新される。
        public bool IsBossStarted { get; set; }
        public bool IsQteEventStarted { get; set; }
        public bool FunnelExpandTrigger { get; set; }
        public QteEventState.Step OrderdQteEventStep { get; set; }
        public bool IsBroken { get; set; } 
    }
}
