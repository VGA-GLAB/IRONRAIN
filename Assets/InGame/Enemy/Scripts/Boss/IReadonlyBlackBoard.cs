namespace Enemy.Boss
{
    /// <summary>
    /// 外部から敵の状態を参照する場合に使用する。
    /// </summary>
    public interface IReadonlyBlackBoard : IOwnerTime
    {
        /// <summary>
        /// QTEを行う位置に立っていうかのフラグ。
        /// </summary>
        public bool IsStandingOnQtePosition { get; }
    }
}
