using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
{
    /// <summary>
    /// キャラクターの情報を各層で共有する。
    /// </summary>
    public class BlackBoard
    {
        public BlackBoard(string name = "")
        {
            ID = System.Guid.NewGuid();
            Name = name;
            ActionPlans = new Queue<ActionPlan>();
            WarpPlans = new Queue<ActionPlan.Warp>();
            MovePlans = new Queue<ActionPlan.Move>();
            LookPlans = new Queue<ActionPlan.Look>();
        }

        // 外部から個体毎の判定が出来る。
        // コンストラクタで生成。
        public System.Guid ID { get; private set; }
        public string Name { get; private set; }

        // 現状ボス戦ではポーズ処理が無いが一応。
        public float PausableDeltaTime => Time.deltaTime;

        // BehaviorTreeの各ノードがActionPlanのインスタンスを確保。
        // 毎フレーム値を書き換えてキューイング、Action側でキューから取り出して処理していく。
        // Updateで書きこまれ、Action側が処理せずともLateUpdateで消える。
        public Queue<ActionPlan> ActionPlans { get; private set; }    // 死亡や攻撃など、選んだ行動のみを渡す。
        public Queue<ActionPlan.Warp> WarpPlans { get; private set; } // Positionの書き換えで移動。
        public Queue<ActionPlan.Move> MovePlans { get; private set; } // 速度で1フレームぶん移動。
        public Queue<ActionPlan.Look> LookPlans { get; private set; } // 任意の方向を向く。

        // Perceptionクラスが書き込む。
        // Startでインスタンスが確保、Updateで値が更新され、Disableでnull。
        public CircleArea Area { get; set; }
        public CircleArea PlayerArea { get; set; }
        public Vector3 PointP { get; set; }
        public Vector3 TransformToPointPDirection { get; set; }
        public float ElapsedTime { get; set; }

        // OverrideOrderクラスが書き込む。
        // Updateで値が更新される。
        public bool IsBossStarted { get; set; }

        // ボス戦開始後、最初にボスの登場演出をするためのフラグ。
        // 現状どういった演出なのか企画書に無いため、デフォルトでtrueにしておくことで登場ステートをスキップする。
        // 本来はPerception層で操作する。
        public bool IsAppearCompleted { get; set; } = true;
    }
}
