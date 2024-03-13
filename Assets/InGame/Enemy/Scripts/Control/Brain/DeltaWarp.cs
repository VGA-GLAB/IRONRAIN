using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// BrainからActionへワープ先を渡す。
    /// </summary>
    public struct DeltaWarp
    {
        public Choice Choice;
        public Vector3 Position;
    }
}
