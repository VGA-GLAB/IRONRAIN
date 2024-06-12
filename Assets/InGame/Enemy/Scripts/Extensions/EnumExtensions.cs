using System;

namespace Enemy.Extensions
{
    public static class EnumExtensions
    {
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
