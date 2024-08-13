using UnityEngine;

namespace Enemy.Boss
{
    /// <summary>
    /// ボスキャラクターのパラメータ
    /// プランナーが弄る。
    /// </summary>
    [System.Serializable]
    public class BossParams : MonoBehaviour, IReadonlyBossParams
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

        // 近距離攻撃
        [System.Serializable]
        public class MeleeAttackSettings
        {
            [Tooltip("攻撃アニメーションの再生間隔。")]
            [Range(0.01f, 10.0f)]
            [SerializeField] private float _rate = 0.01f;

            [Tooltip("この範囲にプレイヤーを捉えると攻撃してくる。")]
            [Min(0)]
            [SerializeField] private float _triggerRange = 8.0f;

            public float Rate => _rate;
            public float TriggerRange => _triggerRange;
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
            [Range(10.0f, 20.0f)]
            [SerializeField] private float _toPlayerFrontMoveSpeed = 10.0f;

            public float ToPlayerFrontMoveSpeed => _toPlayerFrontMoveSpeed;
        }

        // 特に弄る必要ないもの、設定できるが現状必要ないもの。
        [System.Serializable]
        public class OtherSettings
        {
            [Range(0.1f, 1.0f)]
            [Tooltip("プレイヤーに接近する際のホーミング力")]
            [SerializeField] private float _approachHomingPower = 0.5f;

            public float ApproachHomingPower => _approachHomingPower;
        }

        [Header("移動速度の設定")]
        [SerializeField] private MoveSpeedSettings _moveSpeed;

        [Header("近距離攻撃の設定")]
        [SerializeField] private MeleeAttackSettings _meleeAttack;

        [Header("遠距離攻撃の設定")]
        [SerializeField] private RangeAttackSettings _rangeAttack;

        [Header("QTEの設定")]
        [SerializeField] private QteSettings _qte;

        [Space(10)]

        [Header("特に弄る必要ない設定")]
        [SerializeField] private OtherSettings _other;

        public MoveSpeedSettings MoveSpeed => _moveSpeed;
        public MeleeAttackSettings MeleeAttackConfig => _meleeAttack;
        public RangeAttackSettings RangeAttackConfig => _rangeAttack;
        public QteSettings Qte => _qte;
        public OtherSettings Other => _other;
    }
}
