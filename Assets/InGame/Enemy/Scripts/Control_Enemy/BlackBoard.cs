using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
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
            ActionPlans = new Queue<ActionPlan>();
            WarpPlans = new Queue<ActionPlan.Warp>();
            MovePlans = new Queue<ActionPlan.Move>();
            LookPlans = new Queue<ActionPlan.Look>();
            FovEnter = new HashSet<Collider>();
            FovStay = new HashSet<Collider>();
            FovExit = new HashSet<Collider>();
        }

        // 外部から個体毎の判定が出来る。
        // コンストラクタで生成。
        public System.Guid ID { get; private set; }
        public string Name { get; private set; }

        // 外部からポーズの処理を呼ぶと反映される。
        // プレイヤー側でQTEの制御を行う際にタイムスケールが変更される。
        public float PausableDeltaTime => Time.deltaTime * PausableTimeScale;
        public float PausableTimeScale => ProvidePlayerInformation.TimeScale * (IsOrderedPause ? 0 : 1);

        // BehaviorTreeの各ノードがActionPlanのインスタンスを確保。
        // 毎フレーム値を書き換えてキューイング、Action側でキューから取り出して処理していく。
        // Updateで書きこまれ、Action側が処理せずともLateUpdateで消える。
        public Queue<ActionPlan> ActionPlans { get; private set; } // 死亡や攻撃など、選んだ行動のみを渡す。
        public Queue<ActionPlan.Warp> WarpPlans { get; private set; } // Positionの書き換えで移動。
        public Queue<ActionPlan.Move> MovePlans { get; private set; } // 速度で1フレームぶん移動。
        public Queue<ActionPlan.Look> LookPlans { get; private set; } // 任意の方向を向く。

        // EyeSensorクラスが書き込む。
        // Updateで書きこまれ、LateUpdateで消える。
        public HashSet<Collider> FovEnter { get; private set; }
        public HashSet<Collider> FovStay { get; private set; }
        public HashSet<Collider> FovExit { get; private set; }

        // Perceptionクラスが書き込む。
        // Startでインスタンスが確保、Updateで値が更新される。
        public CircleArea Area { get; set; }
        public CircleArea PlayerArea { get; set; }
        public CircleArea Slot { get; set; }

        // Perceptionクラスが書き込む。
        // Updateで値が更新される。
        public Vector3 PlayerPosition { get; set; }
        public Vector3 AreaToSlotDirection { get; set; }
        public float AreaToSlotSqrDistance { get; set; }
        public Vector3 TransformToPlayerDirection { get; set; }
        public float TransformToPlayerDistance { get; set; }
        public bool IsApproachCompleted { get; set; }
        public float LifeTime { get; set; }

        // Perceptionクラスが書き込み、OverrideOrderクラスが必要に応じて上書きする。
        // どちらのクラスもUpdateで値が更新される。
        public bool IsPlayerDetected { get; set; }

        // FireRateクラスが書き込む。
        // Updateで値が更新される。
        public float NextAttackTime { get; set; }

        // HitPointクラスが書き込む。
        // Updateで値が更新される。
        public int Hp { get; set; }
        public int CurrentFrameDamage { get; set; }
        public bool IsDying { get; set; }

        // OverrideOrderクラスが書き込む。
        // Updateで値が更新される。
        public bool OrderedAttackTrigger { get; set; }
        public bool IsOrderedPause { get; set; }

        // BrokenStateクラスもしくはEscapeStateクラスが書き込む。
        // 退場が完了し、後処理を呼んで消しても良い状態のフラグ。
        public bool IsExitCompleted { get; set; }

        // 読み取り専用黒板で使う。
        public bool IsAlive => Hp > 0;
    }
}
