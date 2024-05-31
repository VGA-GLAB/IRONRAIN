namespace Enemy.Control
{
    /// <summary>
    /// 外部から敵の状態を参照する場合に使用する。
    /// </summary>
    public interface IReadonlyBlackBoard
    {
        /// <summary>
        /// 敵が生存しているかを判定。
        /// </summary>
        public bool IsAlive { get; }

        /// <summary>
        /// プレイヤーを検知しているかを判定。
        /// </summary>
        public bool IsPlayerDetected { get; }
    }
}
