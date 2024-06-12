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
    }
}
