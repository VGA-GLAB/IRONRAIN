using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// BrainからActionへワープ先を渡す。
    /// </summary>
    public struct WarpPlan
    {
        public Choice Choice;
        public Vector3 Position;
    }
}
