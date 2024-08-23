using UnityEngine;

namespace Enemy.NPC
{
    /// <summary>
    /// NPCに共通するインターフェース
    /// </summary>
    public interface INpc
    {
        /// <summary>
        /// オブジェクト自体の参照
        /// </summary>
        GameObject GameObject { get; }

        /// <summary>
        /// どのシーケンスに登場するのかを判定
        /// </summary>
        int SequenceID { get; }

        /// <summary>
        /// イベントを実行
        /// </summary>
        public void Play();
    }
}
