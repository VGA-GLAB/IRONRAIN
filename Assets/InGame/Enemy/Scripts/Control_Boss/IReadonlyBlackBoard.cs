using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
{
    /// <summary>
    /// 外部から敵の状態を参照する場合に使用する。
    /// </summary>
    public interface IReadonlyBlackBoard : IOwnerTime
    {
        /// <summary>
        /// 前方向。
        /// Transform型のforwardプロパティで取得できる値と等しい。
        /// </summary>
        public Vector3 Forward { get; }
    }
}
