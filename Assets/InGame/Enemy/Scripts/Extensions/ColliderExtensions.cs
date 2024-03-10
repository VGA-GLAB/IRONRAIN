using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Extensions
{
    public static class ColliderExtensions
    {
        /// <summary>
        /// 複数のタグで判定する。
        /// </summary>
        public static bool CompareTags(this Collider collider, params string[] tags)
        {
            foreach (string tag in tags)
            {
                if (collider.gameObject.CompareTag(tag)) return true;
            }

            return false;
        }

        /// <summary>
        /// 複数のタグで判定する。
        /// </summary>
        public static bool CompareTags(this Collision collision, params string[] tags)
        {
            return collision.collider.CompareTags(tags);
        }
    }
}
