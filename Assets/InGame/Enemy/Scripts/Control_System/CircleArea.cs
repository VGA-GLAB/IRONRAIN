using Enemy.DebugUse;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// xz平面上で円状の範囲を表現する。
    /// y軸の座標は0で固定される。
    /// </summary>
    public class CircleArea
    {
        private Vector3 _point;

        public CircleArea(Vector3 point, float radius)
        {
            Point = point;
            Radius = radius;
        }

        /// <summary>
        /// 中心点
        /// </summary>
        public Vector3 Point 
        { 
            get => _point; 
            set => _point = new Vector3(value.x, 0, value.z);
        }
        /// <summary>
        /// 半径
        /// </summary>
        public float Radius { get; private set; }

        /// <summary>
        /// 円同士の衝突判定
        /// y軸の値が違っていても無視する。
        /// </summary>
        public bool Collision(CircleArea other)
        {
            float a = Point.x - other.Point.x;
            float b = Point.z - other.Point.z;
            float c = Radius + other.Radius;

            return a * a + b * b <= c * c;
        }

        /// <summary>
        /// 他の円と接触する場合にめり込まない丁度の位置を返す。
        /// </summary>
        public Vector3 TouchPoint(CircleArea other)
        {
            Vector3 d = Point - other.Point;
            return other.Point + d.normalized * (Radius + other.Radius);
        }

        /// <summary>
        /// 距離の二乗を返す。
        /// </summary>
        public float SqrMagnitude(CircleArea other)
        {
            return (Point - other.Point).sqrMagnitude;
        }

        /// <summary>
        /// y軸の位置を合わせた座標を返す。
        /// </summary>
        public Vector3 MatchPoint(Vector3 position)
        {
            return Point + Vector3.up * position.y;
        }

        /// <summary>
        /// ギズモに描画
        /// </summary>
        public virtual void DrawOnGizmos()
        {
            Debug.Log("？？？");
            GizmosUtils.WireCircle(Point, Radius, Color.white);
        }

        /// <summary>
        /// ギズモに描画
        /// 任意のオブジェクトに高さを合わせる。
        /// </summary>
        public virtual void DrawOnGizmos(Transform owner)
        {
            if (owner == null)
            {
                DrawOnGizmos();
            }
            else
            {
                Vector3 p = new Vector3(Point.x, owner.position.y, Point.z);
                GizmosUtils.WireCircle(p, Radius, Color.white);
            }
        }
    }
}
