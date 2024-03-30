using UnityEngine;

namespace Enemy.Control.GPT
{
    /// <summary>
    /// 退場した敵の情報を集めておく。
    /// </summary>
    public class ExitSource : ISource<ExitSource.Value, ExitSource.Message>
    {
        public struct Value
        {
            /// <summary>
            /// 死んだ敵の数
            /// </summary>
            public int Dead;
            /// <summary>
            /// 撤退した敵の数
            /// </summary>
            public int Escape;
            /// <summary>
            /// 死んだ敵の合計生存時間
            /// </summary>
            public float Cum;
            /// <summary>
            /// 最速で死んだ敵の生存時間
            /// </summary>
            public float Min;
            /// <summary>
            /// 一番長生きした敵の生存時間
            /// </summary>
            public float Max;
        }

        public struct Message
        {
            /// <summary>
            /// 生存時間
            /// </summary>
            public float LifeTime;
            /// <summary>
            /// 死亡して退場したか
            /// </summary>
            public bool IsDead;
        }

        private Value _source;

        public ExitSource()
        {
            _source.Min = float.MaxValue;
            _source.Max = float.MinValue;
        }

        public Value Source { get => _source; }

        /// <summary>
        /// メッセージを受信をもとに計算
        /// </summary>
        public void Calculate(Message msg)
        {
            if (msg.IsDead) _source.Dead++;
            else _source.Escape++;

            _source.Cum += msg.LifeTime;

            _source.Min = Mathf.Min(_source.Min, msg.LifeTime);
            _source.Max = Mathf.Max(_source.Max, msg.LifeTime);
        }
    }
}
