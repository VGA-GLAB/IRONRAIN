using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// BrainからActionへ回転させ向ける前方向を渡す。
    /// </summary>
    public struct ForwardPlan
    {
        public Choice Choice;
        public Vector3 Value;
    }
}
