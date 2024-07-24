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
    public class BossParams : MonoBehaviour
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
            // 近距離攻撃
            [System.Serializable]
            public class MeleeAttack
            {
                [Header("攻撃間隔")]
                [Min(0.01f)]
                [SerializeField] private float _rate = 0.01f;

                public float Rate => _rate;
            }

            // 遠距離攻撃
            [System.Serializable]
            public class RangeAttack
            {
                [Header("遠距離攻撃タイミングを記述したファイル")]
                [Tooltip("記述されたタイミングをループする。")]
                [SerializeField] private TextAsset _inputBufferAsset;
                [Header("ファイルを使用する")]
                [SerializeField] private bool _useInputBuffer;
                [Header("ファイルを使用しない場合の攻撃間隔")]
                [Min(0.01f)]
                [SerializeField] private float _rate = 0.01f;

                public TextAsset InputBufferAsset => _inputBufferAsset;
                public bool UseInputBuffer => _useInputBuffer;
                public float Rate => _rate;
            }

            [Header("移動速度")]
            [Min(1.0f)]
            [SerializeField] private float _moveSpeed = 12.0f;
            [Header("近距離攻撃")]
            [SerializeField] private MeleeAttack _meleeAttack;
            [Header("遠距離攻撃")]
            [SerializeField] private RangeAttack _rangeAttack;

            public float MoveSpeed => _moveSpeed;
            public MeleeAttack MeleeAttackConfig => _meleeAttack;
            public RangeAttack RangeAttackConfig => _rangeAttack;
        }

        // 必要に応じてプランナー用に外出しする。
        public static class Const
        {
            // 点Pに接近する際のホーミング力。
            public const float HomingPower = 0.5f;
            // エリアの半径を調整するとプレイヤーと重なりにくくなる。
            public static float AreaRadius = 2.1f;
            // エリアの半径を調整するとボスと重なりにくくなる。
            public static float PlayerAreaRadius = 2.7f;
        }

        [Header("ボス戦開始~登場時の設定")]
        [SerializeField] private AppearSettings _appear;
        [Header("戦闘状態の設定")]
        [SerializeField] private BattleSettings _battle;

        public AppearSettings Appear => _appear;
        public BattleSettings Battle => _battle;
    }
}
