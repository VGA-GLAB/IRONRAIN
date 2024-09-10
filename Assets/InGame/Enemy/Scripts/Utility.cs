using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public static class Utility
    {
        /// <summary>
        /// 子オブジェクトを含めたTryGetComponentメソッド
        /// </summary>
        public static bool TryGetComponentInChildren<T>(this Component component, out T result)
        {
            result = component.GetComponentInChildren<T>();
            return result != null;
        }

        /// <summary>
        /// 列挙型の要素数を返す。
        /// </summary>
        public static int Length<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Length;
        }

        /// <summary>
        /// 列挙型の要素を全て返す。
        /// </summary>
        public static Array GetAll<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T));
        }
    }
}
