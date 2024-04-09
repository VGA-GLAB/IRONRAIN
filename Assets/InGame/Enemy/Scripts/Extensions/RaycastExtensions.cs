using System;
using System.Buffers;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy.Extensions
{
    public static class RaycastExtensions
    {
        // レイキャストにヒットする数。
        // 画面に出る当たり判定を持つキャラクターの数に応じて増やす。
        const int Capacity = 16;

        /// <summary>
        /// OverlapSphereNonAllocメソッドのラッパー。
        /// </summary>
        public static void OverlapSphere(in Vector3 origin, float radius, UnityAction<Collider> action, int capacity = Capacity)
        {
            Collider[] results = ArrayPool<Collider>.Shared.Rent(capacity);

            if (Physics.OverlapSphereNonAlloc(origin, radius, results) > 0)
            {
                // レイにヒットしたオブジェクトに対して処理を実行
                foreach (Collider col in results)
                {
                    if (col == null) break;

                    action?.Invoke(col);
                }
            }

            Array.Clear(results, 0, results.Length);
            ArrayPool<Collider>.Shared.Return(results);
        }
    }
}
