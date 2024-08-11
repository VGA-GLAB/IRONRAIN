namespace Enemy
{
    /// <summary>
    /// 複数のクラスから参照される値を定数化しまとめておく
    /// </summary>
    public static class Const
    {
        // 敵のタグ
        public const string EnemyTag = "Enemy";

        // 敵システムのタグ
        public const string EnemySystemTag = "EnemySystem";

        // プレイヤーのタグ
        public const string PlayerTag = "Player";

        // プレイヤーの近接攻撃武器を指定する際の名前
        public const string PlayerMeleeWeaponName = "Melee";

        // プレイヤーの遠距離武器を指定する際の名前
        public const string PlayerRifleWeaponName = "AssaultRifle";
        public const string PlayerLauncherWeaponName = "RocketLauncher";

        // 視界で認識する事が出来るオブジェクトのタグ一覧
        public static readonly string[] ViewTags = { "Player" };
    }
}
