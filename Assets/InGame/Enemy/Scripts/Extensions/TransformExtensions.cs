using UnityEngine;

namespace Enemy.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Vector3の要素をそれぞれの軸へのオフセットとして位置を計算する。
        /// </summary>
        public static Vector3 OffsetPosition(this Transform origin, Transform direction, Vector3 offset)
        {
            Vector3 ox = direction.right * offset.x;
            Vector3 oy = direction.up * offset.y;
            Vector3 oz = direction.forward * offset.z;
            return origin.position + ox + oy + oz;
        }
    }
}