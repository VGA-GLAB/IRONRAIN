namespace Enemy
{
    /// <summary>
    /// 外部から敵キャラクターのパラメータを参照する際に使用する。
    /// </summary>
    public interface IReadonlyEnemyParams
    {
        /// <summary>
        /// 敵の種類を判定する。
        /// </summary>
        public EnemyType Type { get; }
        /// <summary>
        /// 登場するシーケンスを判定する。
        /// </summary>
        public int SequenceID { get; }
    }
}
