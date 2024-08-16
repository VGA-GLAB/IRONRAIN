using Enemy.DebugUse;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 円状のエリア。
    /// 位置は3次元だが、XZ平面上で当たり判定を取る。
    /// </summary>
    public class Area
    {
        public Area(Vector3 point, float radius, int id = -1)
        {
            ID = id;
            Point = point;
            Radius = radius;
        }

        /// <summary>
        /// 識別するために任意で付与できるID。
        /// 当たり判定の計算自体には影響しない。
        /// </summary>
        public int ID { get; private set; }
        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Point { get; set; }
        /// <summary>
        /// 半径
        /// </summary>
        public float Radius { get; private set; }

        /// <summary>
        /// y軸の値は無視し、xz平面上での円同士の衝突判定。
        /// </summary>
        public bool Collision(Area other)
        {
            float a = Point.x - other.Point.x;
            float b = Point.z - other.Point.z;
            float c = Radius + other.Radius;

            return a * a + b * b <= c * c;
        }

        /// <summary>
        /// 接触する場合に、めり込まない丁度の位置を返す。
        /// </summary>
        public Vector3 TouchPoint(Area other)
        {
            Vector3 a = new Vector3(Point.x, 0, Point.z);
            Vector3 b = new Vector3(other.Point.x, 0, other.Point.z);
            Vector3 dir = (a - b).normalized;

            return other.Point + dir * (Radius + other.Radius);
        }

        /// <summary>
        /// 距離の二乗を返す。
        /// </summary>
        public float SqrMagnitude(Area other)
        {
            return (Point - other.Point).sqrMagnitude;
        }

        /// <summary>
        /// ギズモに描画。
        /// 描画する高さを引数で指定できる。
        /// </summary>
        public void Draw()
        {
            GizmosUtils.WireCircle(Point, Radius, Color.white);
        }
    }
}
