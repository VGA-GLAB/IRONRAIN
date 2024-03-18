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
        /// アイドル状態に再生するアニメーション名
        /// </summary>
        public const string IdleAnimationName = "Idle";
        /// <summary>
        /// 攻撃時に再生するアニメーション名
        /// </summary>
        public const string AttackAnimationName = "Attack";
        /// <summary>
        /// 撃破された際に再生するアニメーション名
        /// </summary>
        public const string BrokenAnimationName = "Broken";
        /// <summary>
        /// 左に移動する際に再生するアニメーション名
        /// </summary>
        public const string LeftAnimationName = "Left";
        /// <summary>
        /// 右に移動する際に再生するアニメーション名
        /// </summary>
        public const string RightAnimationName = "Right";

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
    }
}
