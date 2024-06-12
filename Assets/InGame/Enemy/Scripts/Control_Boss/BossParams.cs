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
        // 登場
        [System.Serializable]
        public class AppearSettings
        {
            [Header("移動速度")]
            [Min(1.0f)]
            [SerializeField] private float _moveSpeed = 12.0f;

            public float MoveSpeed => _moveSpeed;
        }

        // 戦闘
        [System.Serializable]
        public class BattleSettings
        {
            [Header("移動速度")]
            [Min(1.0f)]
            [SerializeField] private float _moveSpeed = 12.0f;

            public float MoveSpeed => _moveSpeed;
        }

        // 必要に応じてプランナー用に外出しする。
        public static class Const
        {
            // 点Pに接近する際のホーミング力。
            public const float HomingPower = 0.5f;
            // エリアの半径を調整するとプレイヤーと重なりにくくなる。
            public static float AreaRadius = 0.5f;
            // エリアの半径を調整するとボスと重なりにくくなる。
            public static float PlayerAreaRadius = 0.5f;
        }

        [Header("ボス戦開始~登場時の設定")]
        [SerializeField] private AppearSettings _appear;
        [Header("戦闘状態の設定")]
        [SerializeField] private BattleSettings _battle;

        public AppearSettings Appear => _appear;
        public BattleSettings Battle => _battle;
    }
}
