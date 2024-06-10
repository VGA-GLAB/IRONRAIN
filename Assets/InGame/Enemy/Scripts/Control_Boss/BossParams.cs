using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
{
    /// <summary>
    /// ボスキャラクターのパラメータ
    /// プランナーが弄る。
    /// </summary>
    [System.Serializable]
    public class BossParams
    {
        // 戦闘
        [System.Serializable]
        public class BattleSettings
        {
            //
        }

        // デバッグ用
        // 必要に応じてプランナー用に外出しする。
        public static class Debug
        {
            // エリアの半径を調整するとプレイヤーと重なりにくくなる。
            public static float AreaRadius = 0.5f;
            // エリアの半径を調整するとボスと重なりにくくなる。
            public static float PlayerAreaRadius = 0.5f;
        }
    }
}
