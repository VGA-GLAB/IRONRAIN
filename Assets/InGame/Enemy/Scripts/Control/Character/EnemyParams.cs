using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 敵キャラクター毎のパラメータを一括で管理する。
    /// 必要に応じて各クラスに注入する。
    /// </summary>
    [CreateAssetMenu(fileName ="EnemyParams_" ,menuName ="Enemy/Params")]
    public class EnemyParams : ScriptableObject
    {
        // 視界
        [System.Serializable]
        public class FovSettings
        {
            [Header("半径")]
            [Min(0)]
            [SerializeField] private float _radius = 4.0f;
            [Header("中心位置のオフセット")]
            [SerializeField] private Vector3 _offset;

            public float Radius => _radius;
            public Vector3 Offset => _offset;
        }

        // 位置取りを決めるエリア
        [System.Serializable]
        public class AreaSettings 
        {
            [Header("キャラクターの範囲の半径")]
            [Tooltip("この範囲内にプレイヤーを侵入させないように動く")]
            [Min(1.0f)]
            [SerializeField] private float _radius = 2.0f;
            [Header("プレイヤーの範囲の半径")]
            [Tooltip("この範囲内にキャラクターは侵入しないように動く")]
            [Min(1.0f)]
            [SerializeField] private float _playerRadius = 2.7f;

            public float Radius => _radius;
            public float PlayerRadius=> _playerRadius;
        }

        // 移動
        [System.Serializable]
        public class MoveSettings
        {
            [Header("プレイヤーに向かう速さ")]
            [Tooltip("登場後、この速さでプレイヤーに追従する")]
            [Min(1.0f)]
            [SerializeField] private float _chaseSpeed = 6.0f;

            public float ChaseSpeed => _chaseSpeed;
        }

        // 戦闘
        [System.Serializable]
        public class TacticalSettings
        {
            [Header("最大体力")]
            [Min(1)]
            [SerializeField] private int _maxHp = 10;
            [Range(0, 1.0f)]
            [Header("瀕死状態になる体力(割合)")]
            [SerializeField] private float _dying = 0.2f;
            [Header("生存時間(秒)")]
            [Min(1)]
            [SerializeField] private float _lifeTime;
            [Header("攻撃タイミングを記述したファイル")]
            [Tooltip("記述されたタイミングをループする。")]
            [SerializeField] private TextAsset _inputBufferAsset;
            [Header("ファイルを使用する")]
            [SerializeField] private bool _useInputBuffer;
            [Header("ファイルを使用しない場合の攻撃間隔")]
            [Min(0.01f)]
            [SerializeField] private float _attackRate;
            [Header("ダメージ耐性")]
            [SerializeField] private Armor _armor;

            public int MaxHp => _maxHp;
            public float Dying => _dying;
            public float LifeTime => _lifeTime;
            public TextAsset InputBufferAsset => _inputBufferAsset;
            public bool UseInputBuffer => _useInputBuffer;
            public float AttackRate => _attackRate;
            public Armor Armor => _armor;
        }

        // デバッグ用
        // 必要に応じてプランナー用に外出しする。
        public static class Debug
        {
            public static float HomingPower = 0.5f;
            public static float AttackAnimationPlayTime = 1.0f;
            public static float BrokenAnimationPlayTime = 1.0f;
        }

        [Header("視界の設定")]
        [SerializeField] private FovSettings _fov;
        [Header("侵入不可能な範囲の設定")]
        [SerializeField] private AreaSettings _area;
        [Header("移動の設定")]
        [SerializeField] private MoveSettings _move;
        [Header("戦闘の設定")]
        [SerializeField] private TacticalSettings _tactical;

        public FovSettings FOV => _fov;
        public AreaSettings Area => _area;
        public MoveSettings Move => _move;
        public TacticalSettings Tactical => _tactical;
    }
}