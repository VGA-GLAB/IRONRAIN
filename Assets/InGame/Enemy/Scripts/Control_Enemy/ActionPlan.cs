using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// BrainからActionへ行動ごとの情報を渡す。
    /// </summary>
    public class ActionPlan
    {
        public ActionPlan(Choice choice) => Choice = choice;

        /// <summary>
        /// どの行動を選択したかは共通で必要。
        /// </summary>
        public Choice Choice { get; private set; }

        /// <summary>
        /// Positionの書き換えで移動する。
        /// </summary>
        public class Warp : ActionPlan
        {
            public Warp(Choice choice) : base(choice) { }

            public Vector3 Position { get; set; }
        }

        /// <summary>
        /// 速度で1フレームぶん移動する。
        /// </summary>
        public class Move : ActionPlan
        {
            public Move(Choice choice) : base(choice) { }

            public Vector3 Direction { get; set; }
            public float Speed { get; set; }
        }

        /// <summary>
        /// Y軸の回転で任意の方向を向く。
        /// </summary>
        public class Look : ActionPlan
        {
            public Look(Choice choice) : base(choice) { }

            public Vector3 Forward { get; set; }
        }
    }
}