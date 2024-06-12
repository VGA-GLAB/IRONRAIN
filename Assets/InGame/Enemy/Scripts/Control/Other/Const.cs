namespace Enemy.Control
{
    /// <summary>
    /// 複数のクラスから参照される値を定数化しまとめておく
    /// </summary>
    public static class Const
    {
        /// <summary>
        /// プレイヤーのタグ
        /// </summary>
        public const string PlayerTag = "Player";

        /// <summary>
        /// プレイヤーの近接攻撃武器を指定する際の名前
        /// </summary>
        public const string PlayerMeleeWeaponName = "Melee";
        /// <summary>
        /// プレイヤーの遠距離攻撃武器を指定する際の名前
        /// </summary>
        public const string PlayerRangeWeaponName = "Range";

        /// <summary>
        /// ダメージ判定用のタグ一覧
        /// </summary>
        public static readonly string[] DamageTags =
        {
            "Dummy",
        };

        /// <summary>
        /// 視界で認識する事が出来るオブジェクトのタグ一覧
        /// </summary>
        public static readonly string[] ViewTags =
        {
            "Player",
        };

        /// <summary>
        /// アニメーションのステート周りの定数値
        /// </summary>
        public static class AnimationName
        {
            /// <summary>
            /// 接近開始時に再生するアニメーション名
            /// </summary>
            public const string ApproachEnter = "Advance";
            /// <summary>
            /// 接近途中に再生するアニメーション名
            /// </summary>
            public const string ApproachStay = "AdvanceStay";
            /// <summary>
            /// アイドル状態に再生するアニメーション名
            /// </summary>
            public const string Idle = "Stop";
            /// <summary>
            /// 戦闘中に再生されるブレンドツリー名
            /// </summary>
            public const string Battle = "Battle";
            /// <summary>
            /// 武器を構えるアニメーション名
            /// </summary>
            public const string Aim = "SetUp";
            /// <summary>
            /// 攻撃のアニメーション名
            /// </summary>
            public const string Fire = "Attack";
            /// <summary>
            /// リロードのアニメーション名
            /// </summary>
            public const string Reload = "Recast";
            /// <summary>
            /// 撃破された際のアニメーション名
            /// </summary>
            public const string Broken = "Destroy";
        }

        /// <summary>
        /// アニメーションのパラメータ周りの定数値
        /// </summary>
        public static class AnimationParam
        {
            /// <summary>
            /// アニメーションの再生速度を変更するパラメータ名
            /// </summary>
            public const string PlaySpeed = "PlaySpeed";
            /// <summary>
            /// アニメーションの左右方向の制御用パラメータ名
            /// </summary>
            public const string LeftRight = "X";
            /// <summary>
            /// アニメーションの前後方向の制御用パラメータ名
            /// </summary>
            public const string ForwardBack = "Y";
            /// <summary>
            /// アバターマスクで攻撃準備アニメーションに遷移するトリガー名
            /// 準備 -> 攻撃 と連続して再生される。
            /// </summary>
            public const string AttackTrigger = "AttackTrigger";
            /// <summary>
            /// アバターマスクで攻撃アニメーションのループを終了するトリガー名
            /// </summary>
            public const string AttackEndTrigger = "AttackEndTrigger";
            /// <summary>
            /// ダメージアニメーションに遷移するトリガー名
            /// </summary>
            public const string DamagedTrigger = "DamagedTrigger";
        }
    }
}
