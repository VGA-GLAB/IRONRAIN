using UnityEngine;

namespace Enemy.Boss
{
    // 戦闘位置
    [System.Serializable]
    public class PositionSettings
    {
        [Tooltip("戦闘中、プレイヤーのアイレベルからの高さのオフセット")]
        [SerializeField] private float _heightOffset = -5.0f;

        public Vector3 HeightOffset => Vector3.up * _heightOffset;
    }

    // 移動速度
    [System.Serializable]
    public class MoveSpeedSettings
    {
        [Tooltip("戦闘中、点Pを追従する速度")]
        [Range(1.0f, 100.0f)]
        [SerializeField] private float _chase = 50.0f;

        public float Chase => _chase;
    }

    // 近接攻撃
    [System.Serializable]
    public class MeleeAttackSettings
    {
        [Tooltip("攻撃アニメーションの再生間隔。")]
        [Range(0.01f, 10.0f)]
        [SerializeField] private float _rate = 0.01f;

        [Tooltip("この範囲にプレイヤーを捉えると攻撃してくる。")]
        [Min(0)]
        [SerializeField] private float _triggerRange = 8.0f;

        [Tooltip("ジェットパックのチャージ後、突撃してくる際の速さ")]
        [Min(1.0f)]
        [SerializeField] private float _chargeSpeed = 50.0f;

        public float Rate => _rate;
        public float TriggerRange => _triggerRange;
        public float ChargeSpeed => _chargeSpeed;
    }

    // 遠距離攻撃
    [System.Serializable]
    public class RangeAttackSettings
    {
        [Tooltip("攻撃アニメーションの再生間隔。")]
        [Range(0.01f, 10.0f)]
        [SerializeField] private float _rate = 0.01f;
        [Tooltip("連続で攻撃する最大回数")]
        [Min(1)]
        [SerializeField] private int _maxContinuous = 3;
        [Tooltip("連続で攻撃する最小回数")]
        [Min(1)]
        [SerializeField] private int _minContinuous = 1;

        [Space(10)]

        [Tooltip("入力バッファを使って攻撃タイミングを制御する場合は、Rateで設定した値は無視される。")]
        [SerializeField] private bool _useInputBuffer;
        [SerializeField] private TextAsset _inputBufferAsset;

        public float Rate => _rate;
        public bool UseInputBuffer => _useInputBuffer;
        public TextAsset InputBufferAsset => _inputBufferAsset;
        public int MaxContinuous => _maxContinuous;
        public int MinContinuous => _minContinuous;
    }

    // QTE、左腕破壊
    [System.Serializable]
    public class BreakLeftArmSettings
    {
        [Tooltip("この位置まで近づいた後、左腕を破壊する演出を再生する。")]
        [Min(1.0f)]
        [SerializeField] private float _distance = 10.0f;

        [Tooltip("プレイヤーの正面のレーンに移動後、この速さで左腕破壊アニメーション再生位置まで近づく。")]
        [Min(1.0f)]
        [SerializeField] private float _moveSpeed = 6.0f;

        public float Distance => _distance;
        public float MoveSpeed => _moveSpeed;
    }

    // QTE、鍔迫り合い1回目
    [System.Serializable]
    public class FirstQteSettings
    {
        [Tooltip("鍔迫り合いで吹き飛ばされる際の威力")]
        [Min(1.0f)]
        [SerializeField] private float _knockBack = 15.0f;

        public float KnockBack => _knockBack;
    }

    // QTE、鍔迫り合い2回目
    [System.Serializable]
    public class SecondQteSettings
    {
        [Tooltip("この位置まで近づいた後、鍔迫り合いのアニメーションを再生する。")]
        [Min(1.0f)]
        [SerializeField] private float _distance = 10.0f;

        [Tooltip("1回目のノックバック後、この速さで鍔迫り合いのアニメーション再生位置まで近づく。")]
        [Min(1.0f)]
        [SerializeField] private float _moveSpeed = 6.0f;

        [Tooltip("鍔迫り合いで吹き飛ばされる際の威力")]
        [Min(1.0f)]
        [SerializeField] private float _knockBack = 15.0f;

        public float Distance => _distance;
        public float MoveSpeed => _moveSpeed;
        public float KnockBack => _knockBack;
    }

    // QTE、貫かれて死ぬ
    [System.Serializable]
    public class FinalQteSettings
    {
        //
    }

    /// <summary>
    /// ボスキャラクターのパラメータ
    /// プランナーが弄る。
    /// </summary>
    [System.Serializable]
    public class BossParams : MonoBehaviour, IReadonlyBossParams
    {
        [Header("戦闘位置の設定")]
        [SerializeField] private PositionSettings _position;

        [Header("移動速度の設定")]
        [SerializeField] private MoveSpeedSettings _moveSpeed;

        [Header("近距離攻撃の設定")]
        [SerializeField] private MeleeAttackSettings _meleeAttack;

        [Header("遠距離攻撃の設定")]
        [SerializeField] private RangeAttackSettings _rangeAttack;

        [Header("QTE、左腕破壊演出の設定")]
        [SerializeField] private BreakLeftArmSettings _breakLeftArm;

        [Header("QTE、鍔迫り合い1回目の設定")]
        [SerializeField] private FirstQteSettings _firstQte;

        [Header("QTE、鍔迫り合い2回目の設定")]
        [SerializeField] private SecondQteSettings _secondQte;

        [Header("QTE、撃破される演出の設定")]
        [SerializeField] private FinalQteSettings _finalQte;

        public PositionSettings Position => _position;
        public MoveSpeedSettings MoveSpeed => _moveSpeed;
        public MeleeAttackSettings MeleeAttackConfig => _meleeAttack;
        public RangeAttackSettings RangeAttackConfig => _rangeAttack;
        public BreakLeftArmSettings BreakLeftArm => _breakLeftArm;
        public FirstQteSettings FirstQte => _firstQte;
        public SecondQteSettings SecondQte => _secondQte;
        public FinalQteSettings FinalQte => _finalQte;

        // 雑魚と共通化させるために一応パラメータを設定しておく。
        public int MaxHp => int.MaxValue / 2;
        public Armor Armor => Armor.Invincible;
    }
}
