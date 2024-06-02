using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 黒板からキャラクター毎の時間を取得する用途
    /// </summary>
    public interface IOwnerTime
    {
        /// <summary>
        /// ポーズに対応した単位時間
        /// </summary>
        float PausableDeltaTime { get; }
    }
}
