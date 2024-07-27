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
    /// 特殊な状態を設定し、行動を制限するための列挙型。
    /// </summary>
    public enum SpecialCondition
    {
        None,         // 特に行動制限なし、デフォルト
        ManualAttack, // 外部からメソッドを呼ぶことでのみ、攻撃する。
    }

    /// <summary>
    /// 敵キャラクター個体毎のパラメータ。
    /// プランナーが弄る。
    /// </summary>
    public class EnemyParams : MonoBehaviour, IReadonlyEnemyParams
    {
        // 移動速度
        [System.Serializable]
        public class MoveSpeedSettings
        {
            [Tooltip("画面上に表示され、プレイヤーの正面に向かって移動中の速度。")]
            [Range(1.0f, 100.0f)]
            [SerializeField] private float _approach = 50.0f;

            [Tooltip("プレイヤーの正面に移動完了、攻撃しつつ移動する際の速度。")]
            [Range(1.0f, 100.0f)]
            [SerializeField] private float _chase = 50.0f;

            [Tooltip("生存時間を越え、画面外に撤退する際の速度。")]
            [Range(1.0f, 100.0f)]
            [SerializeField] private float _exit = 50.0f;

            public float Approach => _approach;
            public float Chase => _chase;
            public float Exit => _exit;
        }

        // 攻撃
        [System.Serializable]
        public class AttackSettings
        {
            [Tooltip("この範囲にプレイヤーを捉えると攻撃してくる。")]
            [Min(0)]
            [SerializeField] private float _triggerRange = 8.0f;

            [Tooltip("攻撃アニメーションの再生間隔。")]
            [Range(0.01f, 10.0f)]
            [SerializeField] private float _rate = 0.01f;

            [Space(10)]

            [Tooltip("入力バッファを使って攻撃タイミングを制御する場合は、Rateで設定した値は無視される。")]
            [SerializeField] private bool _useInputBuffer;
            [SerializeField] private TextAsset _inputBufferAsset;

            public float TriggerRange => _triggerRange;
            public float Rate => _rate;
            public bool UseInputBuffer => _useInputBuffer;
            public TextAsset InputBufferAsset => _inputBufferAsset;
        }

        // 特に弄る必要ないもの、設定できるが現状必要ないもの。
        [System.Serializable]
        public class OtherSettings
        {
            [Range(0.1f, 1.0f)]
            [Tooltip("プレイヤーに接近する際のホーミング力")]
            [SerializeField] private float _approachHomingPower = 0.5f;

            [Range(0.1f, 100.0f)]
            [Tooltip("前後左右に移動するアニメーションのブレンドする強さ")]
            [SerializeField] private float _animationBlendPower = 100.0f;

            [Range(0.1f, 1.0f)]
            [Tooltip("接近完了とみなす距離の閾値")]
            [SerializeField] private float _approachCompleteThreshold = 0.1f;

            [Range(1.0f, 10.0f)]
            [Tooltip("戦闘時に上下移動する際の速さ")]
            [SerializeField] private float _verticalMoveSpeed = 1.5f;

            [Range(0, 1.0f)]
            [Tooltip("体力がこの割合を下回った場合は瀕死状態になる。")]
            [SerializeField] private float _dying = 0.2f;

            [Range(10.0f, 200.0f)]
            [Tooltip("撤退時、プレイヤーとの距離がこの値を超えると画面から消える。")]
            [SerializeField] private float _offScreenDistance = 100.0f;

            [Tooltip("死亡アニメーションを再生開始から、ステートマシンを止めるまでのディレイ(秒)")]
            [SerializeField] private float _brokenDelay = 2.5f;

            [Tooltip("種類ごとに共通したパラメータ。気持ち程度のFlyWeight。")]
            [SerializeField] private CommonParams _common;

            public float ApproachHomingPower => _approachHomingPower;
            public float AnimationBlendPower => _animationBlendPower;
            public float ApproachCompleteThreshold => _approachCompleteThreshold;
            public float VerticalMoveSpeed => _verticalMoveSpeed;
            public float Dying => _dying;
            public float OffScreenDistance => _offScreenDistance;
            public float BrokenDelay => _brokenDelay;
            public CommonParams Common => _common;
        }

        [Header("スロット番号")]
        [SerializeField] private SlotPool.Place _slotPlace;

        [Header("登場するシーケンス")]
        [SerializeField] private EnemyManager.Sequence _sequence;

        [Min(1)]
        [Header("体力の最大値")]
        [SerializeField] private int _maxHp = 100;

        [Min(1)]
        [Header("生存時間")]
        [Tooltip("プレイヤーの正面に移動完了した後、計測開始する。")]
        [SerializeField] private int _lifeTime = 60;

        [Header("移動速度の設定")]
        [SerializeField] private MoveSpeedSettings _moveSpeed;

        [Header("攻撃の設定")]
        [SerializeField] private AttackSettings _attack;

        [Header("特殊な状態の設定")]
        [SerializeField] private SpecialCondition _specialCondition;

        [Space(10)]

        [Header("特に弄る必要ない設定")]
        [SerializeField] private OtherSettings _other;

        public SlotPool.Place SlotPlace => _slotPlace;
        public int MaxHp => _maxHp;
        public int LifeTime => _lifeTime;
        public MoveSpeedSettings MoveSpeed => _moveSpeed;
        public AttackSettings Attack => _attack;
        public SpecialCondition SpecialCondition => _specialCondition;
        public OtherSettings Other => _other;
        public CommonParams Common => _other.Common;

        // 視界に入った敵を攻撃するという処理になっている都合上、同じ値を参照するようにしている。
        // 攻撃以外にも視界の用途が出来た場合は専用のパラメータを用意し、攻撃範囲と分ける必要あり。
        public float FovRadius => _attack.TriggerRange;

        // インターフェースで外部から参照する。
        public EnemyType Type => _other.Common != null ? _other.Common.Type : EnemyType.Dummy;
        public EnemyManager.Sequence Sequence => _sequence;
    }
}