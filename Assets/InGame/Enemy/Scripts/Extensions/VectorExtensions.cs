using UnityEngine;

namespace Enemy.Extensions
{
    public static class VectorExtensions
    {
        /// <summary>
        /// aをbに射影したベクトルを返す。
        /// </summary>
        public static Vector2 VectorProjection(this Vector2 a, in Vector2 b)
        {
            Vector2 nor = b.normalized;
            return nor * Vector2.Dot(a, nor);
        }
    }
}
