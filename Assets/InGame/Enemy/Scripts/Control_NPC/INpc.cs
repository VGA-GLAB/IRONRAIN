namespace Enemy.Control
{
    /// <summary>
    /// NPCに共通するインターフェース
    /// </summary>
    public interface INpc
    {
        /// <summary>
        /// どのシーケンスに登場するのかを判定
        /// </summary>
        EnemyManager.Sequence Sequence { get; }

        /// <summary>
        /// イベントを実行
        /// </summary>
        public void Play();
    }
}
