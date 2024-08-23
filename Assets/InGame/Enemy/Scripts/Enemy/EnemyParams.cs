using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 敵の種類を判定するための列挙型。
    /// </summary>
    public enum EnemyType
    {
        Dummy,      // 未割当の場合に返る値
        Assault, // 銃持ち
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

    // スロット
    [System.Serializable]
    public class SlotSettings
    {
        [Tooltip("プレイヤーとの位置関係")]
        [SerializeField] private SlotPlace _place = SlotPlace.Middle;
        [Tooltip("プレイヤーとの距離")]
        [Range(1.0f, 50.0f)]
        [SerializeField] private float _forwardOffset = 25.0f;

        public SlotPlace Place => _place;
        public float ForwardOffset => _forwardOffset;
    }

    // 移動速度
    [System.Serializable]
    public class MoveSpeedSettings
    {
        [Tooltip("画面上に表示され、プレイヤーの正面に向かって移動中の速度。")]
        [Min(1.0f)]
        [SerializeField] private float _approach = 1.0f;

        [Tooltip("プレイヤーの正面に移動完了、攻撃しつつ移動する際の速度。")]
        [Min(1.0f)]
        [SerializeField] private float _chase = 1.0f;

        [Tooltip("生存時間を越え、画面外に撤退する際の速度。")]
        [Min(1.0f)]
        [SerializeField] private float _exit = 1.0f;

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

    /// <summary>
    /// 敵キャラクター個体毎のパラメータ。
    /// プランナーが弄る。
    /// </summary>
    public class EnemyParams : MonoBehaviour, IReadonlyEnemyParams
    {
        [Header("スロットの設定")]
        [SerializeField] private SlotSettings _slot;

        [Header("登場するシーケンスのID")]
        [SerializeField] private int _sequenceID;

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

        [SerializeField] private CommonParams _common;

        public SlotSettings Slot => _slot;
        public int MaxHp => _maxHp;
        public int LifeTime => _lifeTime;
        public MoveSpeedSettings MoveSpeed => _moveSpeed;
        public AttackSettings Attack => _attack;
        public SpecialCondition SpecialCondition => _specialCondition;
        public CommonParams Common => _common;

        // 視界に入った敵を攻撃するという処理になっている都合上、同じ値を参照するようにしている。
        // 攻撃以外にも視界の用途が出来た場合は専用のパラメータを用意し、攻撃範囲と分ける必要あり。
        public float FovRadius => _attack.TriggerRange;

        // インターフェースで外部から参照する。
        public EnemyType Type => Common != null ? Common.Type : EnemyType.Dummy;
        public int SequenceID => _sequenceID;

        // インスペクターでパラメータ弄った場合に勝手に名前を変更する。
        private void OnValidate()
        {
            string slotID = "";
            if (_slot.Place == SlotPlace.Left) slotID = "l";
            else if (_slot.Place == SlotPlace.MiddleLeft) slotID = "ml";
            else if (_slot.Place == SlotPlace.Middle) slotID = "m";
            else if (_slot.Place == SlotPlace.MiddleRight) slotID = "mr";
            else if (_slot.Place == SlotPlace.Right) slotID = "r";

            string weaponID = "";
            if (TryGetComponent(out AssaultEquipment _)) weaponID = "Assault";
            else if (TryGetComponent(out LauncherEquipment _)) weaponID = "Launcher";
            else if (TryGetComponent(out ShieldEquipment _)) weaponID = "Shield";

            name = $"Enemy_{SequenceID}_{weaponID}_{slotID}";
        }
    }
}