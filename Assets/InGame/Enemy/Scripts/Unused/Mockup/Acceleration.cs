#if false

using Enemy.Extensions;
using UnityEngine;

namespace Enemy.Unused
{
    public static class Acceleration
    {
        /// <summary>
        /// 自身と目標が水平に並ぶように移動を行うベクトルを返す。
        /// </summary>
        public static Vector3 Horizontal(in Vector3 self, in Vector3 target, in Vector3 targetForward, in Vector3 targetRight,
             in Vector3 velocity, float forwardOffset, float approximately, float weight)
        {
            // y軸を無視してxz平面上で計算
            Vector3 s = new Vector3(self.x, 0, self.z);
            Vector3 t = new Vector3(target.x, 0, target.z);

            // 射影するベクトルの原点
            Vector3 origin = t + targetForward * forwardOffset;

            // 自身のy軸を無視した位置を、原点から右向きのベクトルに射影する
            Vector3 right = origin + targetRight;
            Vector3 vp = VectorProjection(s - right, origin - right) + right;

            // 震えるのを防ぐ
            // ほぼ等しい距離まで来ていたら停止するように逆方向の速度ベクトルを返す。
            if ((vp - s).sqrMagnitude < approximately)
            {
                return -velocity;
            }
            else
            {
                return (vp - s).normalized * weight;
            }
        }

        /// <summary>
        /// 近づき過ぎた場合に逆方向に移動するベクトルを返す。
        /// </summary>
        public static Vector3 Avoid(in Vector3 self, in Vector3 target, float distance, float weight)
        {
            // y軸を無視してxz平面上で計算
            Vector3 s = new Vector3(self.x, 0, self.z);
            Vector3 t = new Vector3(target.x, 0, target.z);

            Vector3 v = s - t;
            if (v.sqrMagnitude < distance * distance)
            {
                return v.normalized * weight;
            }
            else return Vector3.zero;
        }

        /// <summary>
        /// 自身を目標の方向に一定割合向けるベクトルを返す。
        /// </summary>
        public static Vector3 Homing(in Vector3 self, in Vector3 target, in Vector3 forward, float power, float weight)
        {
            // y軸を無視してxz平面上で計算
            Vector3 s = new Vector3(self.x, 0, self.z);
            Vector3 t = new Vector3(target.x, 0, target.z);

            Vector3 dir = (t - s).normalized;
            Vector3 f = Vector3.Lerp(forward, dir, power);
            return f * weight;
        }

        /// <summary>
        /// 上下移動するようなベクトルを返す。
        /// </summary>
        public static Vector3 Vertical(in Vector3 self, in Vector3 target, float power, float weight)
        {
            float range = Mathf.Abs(self.y - target.y);
            float lerp = Mathf.Lerp(0, range, power);

            return Mathf.Sign(target.y - self.y) * Vector3.up * lerp * weight;
        }

        // xz平面上でaをbに射影したベクトルを返す。
        static Vector3 VectorProjection(in Vector3 a, in Vector3 b)
        {
            // y軸は無視
            Vector2 aa = new Vector2(a.x, a.z);
            Vector2 bb = new Vector2(b.x, b.z);

            Vector2 vp = aa.VectorProjection(bb);
            return new Vector3(vp.x, 0, vp.y);
        }
    }
}

#endif