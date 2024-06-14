using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 敵の種類を判定するための列挙型。
    /// </summary>
    public enum EnemyType
    {
        Dummy,      // 未割当の場合に返る値
        MachineGun, // 銃持ち
        Launcher,   // ランチャー持ち
        Shield,     // シールド持ち
    }

    /// <summary>
    /// 敵キャラクター個体毎のパラメータ。
    /// プランナーが弄る。
    /// </summary>
    [System.Serializable]
    public class EnemyParams : IReadonlyEnemyParams
    {
        // 接近
        [System.Serializable]
        public class AdvanceSettings
        {
            [Header("検知距離")]
            [Min(1.0f)]
            [SerializeField] private float _distance = 33.0f;
            [Header("移動速度")]
            [Min(1.0f)]
            [SerializeField] private float _moveSpeed = 12.0f;
            [Header("スロット番号")]
            [SerializeField] private SlotPool.Place _slotPlace;

            public float Distance => _distance;
            public float MoveSpeed => _moveSpeed;
            public SlotPool.Place SlotPlace => _slotPlace;
        }

        // 戦闘
        [System.Serializable]
        public class BattleSettings
        {
            [Header("最大体力")]
            [Min(1)]
            [SerializeField] private int _maxHp = 10;
            [Range(0, 1.0f)]
            [Header("瀕死状態になる体力(割合)")]
            [SerializeField] private float _dying = 0.2f;
            [Header("生存時間(秒)")]
            [Min(1)]
            [SerializeField] private float _lifeTime = 60.0f;
            [Header("攻撃タイミングを記述したファイル")]
            [Tooltip("記述されたタイミングをループする。")]
            [SerializeField] private TextAsset _inputBufferAsset;
            [Header("ファイルを使用する")]
            [SerializeField] private bool _useInputBuffer;
            [Header("ファイルを使用しない場合の攻撃間隔")]
            [Min(0.01f)]
            [SerializeField] private float _attackRate = 0.01f;
            [Header("プレイヤーに向かう速さ")]
            [Tooltip("登場後、この速さでプレイヤーに追従する")]
            [Min(1.0f)]
            [SerializeField] private float _chaseSpeed = 6.0f;
            [Header("戦闘から撤退する際の速さ")]
            [Min(1.0f)]
            [SerializeField] private float _escapeSpeed = 6.0f;
            [Header("視界の半径")]
            [Tooltip("この範囲にプレイヤーを捉えると攻撃してくる。")]
            [Min(0)]
            [SerializeField] private float _fovRadius = 8.0f;

            public int MaxHp => _maxHp;
            public float Dying => _dying;
            public float LifeTime => _lifeTime;
            public TextAsset InputBufferAsset => _inputBufferAsset;
            public bool UseInputBuffer => _useInputBuffer;
            public float AttackRate => _attackRate;
            public float ChaseSpeed => _chaseSpeed;
            public float EscapeSpeed => _escapeSpeed;
            public float FovRadius => _fovRadius;
        }

        // その他の設定
        [System.Serializable]
        public class OtherSettings
        {
            [Header("チュートリアルの敵として扱う")]
            [Tooltip("外部から攻撃命令をされた場合のみ攻撃する。")]
            [SerializeField] private bool _isTutorial;
            [Header("シーケンス設定")]
            [SerializeField] private EnemyManager.Sequence _sequence;

            public bool IsTutorial => _isTutorial;
            public EnemyManager.Sequence Sequence => _sequence;
        }

        // 必要に応じてプランナー用に外出しする。
        public static class Const
        {
            // プレイヤーに接近する際のホーミング力
            public const float HomingPower = 0.5f;
            // 前後左右に移動するアニメーションのブレンドする強さ
            public const float BlendTreeParameterMag = 100.0f;
            // 接近完了とみなす距離の閾値
            public const float ApproachCompleteThreshold = 0.1f;
            // 戦闘時に上下移動する際の速さ
            public const float VerticalMoveSpeed = 1.5f;
        }

        [Header("生成~直進状態の設定")]
        [SerializeField] private AdvanceSettings _advance;
        [Header("戦闘状態の設定")]
        [SerializeField] private BattleSettings _battle;
        [Header("その他の設定")]
        [SerializeField] private OtherSettings _other;
        [Header("種類ごとに共通したパラメータ")]
        [Tooltip("例外的なキャラクターがいない場合はこの項目は弄らなくて良い。")]
        [SerializeField] private CommonParams _common;

        public AdvanceSettings Advance => _advance;
        public BattleSettings Battle => _battle;
        public OtherSettings Other => _other;
        public CommonParams Common => _common;

        // インターフェースで外部から参照する。
        public EnemyType Type => _common != null ? _common.Type : EnemyType.Dummy;
        public EnemyManager.Sequence Sequence => _other.Sequence;
    }
}