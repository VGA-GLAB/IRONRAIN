using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 敵キャラクター個体毎のパラメータ。
    /// プランナーが弄る。
    /// </summary>
    [System.Serializable]
    public class EnemyParams
    {
        // 接近
        [System.Serializable]
        public class AdvanceSettings
        {
            [Header("レーンの位置")]
            [SerializeField] private SlotPlace _slot;
            [Header("検知距離")]
            [Min(1.0f)]
            [SerializeField] private float _distance = 33.0f;
            [Header("移動速度")]
            [Min(1.0f)]
            [SerializeField] private float _moveSpeed = 12.0f;

            public SlotPlace Slot => _slot;
            public float Distance => _distance;
            public float MoveSpeed => _moveSpeed;
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

        // デバッグ用
        // 必要に応じてプランナー用に外出しする。
        public static class Debug
        {
            // プレイヤーに接近する際のホーミング力
            public static float HomingPower = 0.5f;
            // 前後左右に移動するアニメーションのブレンドする強さ
            public static float BlendTreeParameterMag = 100.0f;
            // 接近完了とみなす距離の閾値
            public static float ApproachCompleteThreshold = 0.1f;
        }

        [Header("生成~直進状態の設定")]
        [SerializeField] private AdvanceSettings _advance;
        [Header("戦闘状態の設定")]
        [SerializeField] private BattleSettings _battle;
        [Header("種類ごとに共通したパラメータ")]
        [Tooltip("例外的なキャラクターがいない場合はこの項目は弄らなくて良い。")]
        [SerializeField] private CommonParams _common;

        public AdvanceSettings Advance => _advance;
        public BattleSettings Battle => _battle;
        public CommonParams Common => _common;
    }
}