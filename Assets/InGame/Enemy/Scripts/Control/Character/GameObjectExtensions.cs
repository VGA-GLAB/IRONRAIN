using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Enemy.Control
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// ランダムな名前を付ける。
        /// </summary>
        public static void RandomName(this GameObject gameObject)
        {
            const int Length = 6; // 名前の長さは適当
            const string Table = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            StringBuilder b = new StringBuilder("_", 7);
            for (int i = 0; i < Length; i++)
            {
                b.Append(Table[Random.Range(0, Table.Length)]);
            }

            gameObject.name += b.ToString();
        }
    }
}