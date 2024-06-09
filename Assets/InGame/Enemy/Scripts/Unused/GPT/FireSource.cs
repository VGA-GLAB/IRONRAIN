namespace Enemy.Unused.GPT
{
    /// <summary>
    /// 発射された弾の情報を集めておく。
    /// </summary>
    public class FireSource : ISource<FireSource.Value, FireSource.Message>
    {
        public struct Value
        {
            /// <summary>
            /// 発射した弾の累計
            /// </summary>
            public int Cum;
            /// <summary>
            /// 当たった弾の数
            /// </summary>
            public int Hit;
        }

        public struct Message
        {
            /// <summary>
            /// 弾がヒットしたか
            /// </summary>
            public bool IsHit;
        }

        private Value _source;

        public Value Source { get => _source; }

        /// <summary>
        /// メッセージを受信をもとに計算
        /// </summary>
        public void Calculate(Message msg)
        {
            _source.Cum++;
            if (msg.IsHit) _source.Hit++;
        }
    }
}
