using UnityEngine;

namespace Enemy.DebugUse
{
    /// <summary>
    /// ギズモへの描画はこのクラスを使うこと
    /// </summary>
    public static class GizmosUtils
    {
        /// <summary>
        /// 円の描画
        /// </summary>
        public static void WireCircle(Vector3 position, float radius, Color color)
        {
            Gizmos.color = color;
            Utils.GizmosExtensions.DrawWireCircle(position, radius);
        }

        /// <summary>
        /// 球の描画
        /// </summary>
        public static void WireSphere(Vector3 position, float radius, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(position, radius);
        }

        /// <summary>
        /// 塗りつぶされた球の描画
        /// </summary>
        public static void Sphere(Vector3 position, float radius, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(position, radius);
        }

        /// <summary>
        /// 線の描画
        /// </summary>
        public static void Line(Vector3 a, Vector3 b, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(a, b);
        }

        /// <summary>
        /// 矢印の描画
        /// </summary>
        public static void Arrow(Vector3 a, Vector3 b, Color color)
        {
            Gizmos.color = color;
            Utils.GizmosExtensions.DrawArrow(a, b, arrowHeadLength: 2.0f);
        }

        /// <summary>
        /// 平面の描画
        /// </summary>
        public static void Plane(Vector3 center, float width, float height, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawCube(center, new Vector3(width, height, 0.01f));
        }
    }
}
