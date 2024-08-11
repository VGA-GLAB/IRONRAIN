using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Extensions
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// 子オブジェクトを含めたTryGetComponentメソッド
        /// </summary>
        public static bool TryGetComponentInChildren<T>(this Component component, out T result)
        {
            result = component.GetComponentInChildren<T>();
            return result != null;
        }
    }
}
