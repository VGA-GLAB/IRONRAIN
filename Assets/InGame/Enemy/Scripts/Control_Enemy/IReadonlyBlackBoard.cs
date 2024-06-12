﻿namespace Enemy.Control
{
    /// <summary>
    /// 外部から敵の状態を参照する場合に使用する。
    /// </summary>
    public interface IReadonlyBlackBoard : IOwnerTime
    {
        /// <summary>
        /// 敵が生存しているかを判定。
        /// </summary>
        public bool IsAlive { get; }

        /// <summary>
        /// プレイヤーを検知しているかを判定。
        /// </summary>
        public bool IsPlayerDetected { get; }

        /// <summary>
        /// プレイヤーへの接近が完了したかを判定。
        /// これ以降、攻撃処理を外部から呼ぶことで、任意のタイミングで攻撃させることが出来る。
        /// </summary>
        public bool IsApproachCompleted { get; }
    }
}
