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

    }
}
