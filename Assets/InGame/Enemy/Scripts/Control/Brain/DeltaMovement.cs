using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// BrainからActionへ移動量を渡す。
    /// </summary>
    public struct DeltaMovement
    {
        public Choice Choice;
        public Vector3 Direction;
        public float Speed;
    }
}
