using UnityEngine;

namespace Enemy.Boss
{
    // 移動速度
    [System.Serializable]
    public class MoveSpeedSettings
    {
        [Tooltip("戦闘中、点Pを追従する速度")]
        [Range(1.0f, 100.0f)]
        [SerializeField] private float _chase = 50.0f;

        public float Chase => _chase;
    }

    // 技ごとの設定の基底クラス
    public class Skill
    {
        [Min(0)]
        [SerializeField] private int _damage = 1;

        [Tooltip("チャージ後、この距離までプレイヤーとの間合いを詰めてから攻撃する。")]
        [Range(1.0f, 5.0f)]
        [SerializeField] private float _distance = 4.0f;

        public int Damage => _damage;
        public float SprDistance => _distance * _distance;
        public virtual string ID => nameof(Skill);
    }

    // 下段斬り
    [System.Serializable]
    public class GedanGiri : Skill
    {
        public override string ID => nameof(GedanGiri);
    }

    // 溜め突き
    [System.Serializable]
    public class ChargeThrust : Skill
    {
        public override string ID => nameof(ChargeThrust);
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

        [Header("下段斬り")]
        [SerializeField] private GedanGiri _gedanGiri;
        [Header("溜め突き")]
        [SerializeField] private ChargeThrust _chargeThrust;

        public float Rate => _rate;
        public float TriggerRange => _triggerRange;
        public float ChargeSpeed => _chargeSpeed;
        public GedanGiri GedanGiri => _gedanGiri;
        public ChargeThrust ChargeThrust => _chargeThrust;
    }

    // 遠距離攻撃
    [System.Serializable]
    public class RangeAttackSettings
    {
        [Tooltip("攻撃アニメーションの再生間隔。")]
        [Range(0.01f, 10.0f)]
        [SerializeField] private float _rate = 0.01f;

        [Space(10)]

        [Tooltip("入力バッファを使って攻撃タイミングを制御する場合は、Rateで設定した値は無視される。")]
        [SerializeField] private bool _useInputBuffer;
        [SerializeField] private TextAsset _inputBufferAsset;

        public float Rate => _rate;
        public bool UseInputBuffer => _useInputBuffer;
        public TextAsset InputBufferAsset => _inputBufferAsset;
    }

    // QTE
    [System.Serializable]
    public class QteSettings
    {
        [Tooltip("プレイヤーの正面に移動する際の速さ")]
        [Range(10.0f, 30.0f)]
        [SerializeField] private float _toPlayerFrontMoveSpeed = 10.0f;

        [Tooltip("プレイヤーと殴り合う距離。この距離まで近づいた後に各QTEを行う。")]
        [Range(1.0f, 10.0f)]
        [SerializeField] private float _socialDistance = 20.0f;

        [Tooltip("鍔迫り合いで吹き飛ばされる力")]
        [Range(1.0f, 30.0f)]
        [SerializeField] private float _knockBackPower = 15.0f;

        [Tooltip("鍔迫り合いで吹き飛ばされた後、再び突っ込んでくる速さ")]
        [Range(1.0f, 30.0f)]
        [SerializeField] private float _chargeSpeed = 15.0f;

        public float ToPlayerFrontMoveSpeed => _toPlayerFrontMoveSpeed;
        public float SocialSqrDistance => _socialDistance * _socialDistance;
        public float KnockBackPower => _knockBackPower;
        public float ChargeSpeed => _chargeSpeed;
    }

    /// <summary>
    /// ボスキャラクターのパラメータ
    /// プランナーが弄る。
    /// </summary>
    [System.Serializable]
    public class BossParams : MonoBehaviour, IReadonlyBossParams
    {
        [Header("移動速度の設定")]
        [SerializeField] private MoveSpeedSettings _moveSpeed;

        [Header("近距離攻撃の設定")]
        [SerializeField] private MeleeAttackSettings _meleeAttack;

        [Header("遠距離攻撃の設定")]
        [SerializeField] private RangeAttackSettings _rangeAttack;

        [Header("QTEの設定")]
        [SerializeField] private QteSettings _qte;

        public MoveSpeedSettings MoveSpeed => _moveSpeed;
        public MeleeAttackSettings MeleeAttackConfig => _meleeAttack;
        public RangeAttackSettings RangeAttackConfig => _rangeAttack;
        public QteSettings Qte => _qte;

        // 雑魚と共通化させるために一応パラメータを設定しておく。
        public int MaxHp => int.MaxValue / 2;
        public Armor Armor => Armor.Invincible;
    }
}
