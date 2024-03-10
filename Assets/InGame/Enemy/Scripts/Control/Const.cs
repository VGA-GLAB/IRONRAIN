using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 複数のクラスから参照される値を定数化しまとめておく
    /// </summary>
    public static class Const
    {
        /// <summary>
        /// プレイヤーのタグ
        /// </summary>
        public const string PlayerTag = "Player";

        /// <summary>
        /// ダメージ判定用のタグ一覧
        /// </summary>
        public static string[] DamageTags =
        {
            "Dummy",
        };

        /// <summary>
        /// 視界で認識する事が出来るオブジェクトのタグ一覧
        /// </summary>
        public static string[] ViewTags =
        {
            "Player",
        };
    }
}
