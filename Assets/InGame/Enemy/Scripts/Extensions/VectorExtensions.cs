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

        /// <summary>
        /// 自身を目標の方向に一定割合向けるベクトルを返す。
        /// </summary>
        public static Vector3 Homing(in Vector3 self, in Vector3 target, in Vector3 forward, float power)
        {
            // y軸を無視してxz平面上で計算
            Vector3 s = new Vector3(self.x, 0, self.z);
            Vector3 t = new Vector3(target.x, 0, target.z);

            Vector3 dir = (t - s).normalized;
            // forwardからdirに向けてpowerの値で補間する。
            return Vector3.Lerp(forward, dir, power);
        }
    }
}
