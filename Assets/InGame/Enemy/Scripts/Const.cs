namespace Enemy
{
    /// <summary>
    /// 複数のクラスから参照される値を定数化しまとめておく
    /// </summary>
    public static class Const
    {
        // タグ
        public const string EnemyTag = "Enemy";
        public const string EnemySystemTag = "EnemySystem";
        public const string PlayerTag = "Player";

        // 視界で認識する事が出来るオブジェクトのタグ一覧
        public static readonly string[] ViewTags = { "Player" };

        // プレイヤーの武器を指定する際の名前
        public const string PlayerMeleeWeaponName = "Melee";
        public const string PlayerRifleWeaponName = "AssaultRifle";
        public const string PlayerLauncherWeaponName = "RocketLauncher";
        public const string PlayerMissileWeaponName = "xxx"; // プレイヤー側で要修正。

        // 銃持ちの雑魚のAnimatorControllerのステート名
        public static class Assault
        {
            public const string Idle = "Idle";
            public const string MoveFrontLoop = "all_move_front_lp";
            public const string MoveFrontEnd = "all_move_front_ed";
            public const string MoveLeftStart = "all_move_left_st";
            public const string MoveLeftLoop = "all_move_left_lp";
            public const string MoveLeftEnd = "all_move_left_ed";
            public const string MoveRightStart = "all_move_right_st";
            public const string MoveRightLoop = "all_move_right_lp";
            public const string MoveRightEnd = "all_move_right_ed";
            public const string HoldStart = "enemy_assault_hold_st"; // 武器構え
            public const string HoldLoop = "enemy_assault_hold_lp";  // 武器構え
            public const string FireLoop = "enemy_assault_fire_lp";  // 攻撃
            public const string Damage = "enemy_break00";
        }

        // ランチャー持ちの雑魚のAnimatorControllerのステート名
        public static class Launcher
        {
            public const string Idle = "Idle";
            public const string MoveFrontLoop = "all_move_front_lp";
            public const string MoveFrontEnd = "all_move_front_ed";
            public const string MoveLeftStart = "all_move_left_st";
            public const string MoveLeftLoop = "all_move_left_lp";
            public const string MoveLeftEnd = "all_move_left_ed";
            public const string MoveRightStart = "all_move_right_st";
            public const string MoveRightLoop = "all_move_right_lp";
            public const string MoveRightEnd = "all_move_right_ed";
            public const string HoldStart = "enemy_rocket_hold_st"; // 武器構え
            public const string HoldLoop = "enemy_rocket_hold_lp";  // 武器構え
            public const string Fire = "enemy_rocket_fire";         // 攻撃(アニメーション3分割なし)
            public const string Reload = "enemy_rocket_reload";     // 武器をリロード
            public const string Damage = "enemy_break00";
        }

        // 盾持ちの雑魚のAnimatorControllerのステート名
        public static class Shield
        {
            public const string Idle = "Idle";
            public const string MoveFrontLoop = "all_move_front_lp";
            public const string MoveFrontEnd = "all_move_front_ed";
            public const string MoveLeftStart = "all_move_left_st";
            public const string MoveLeftLoop = "all_move_left_lp";
            public const string MoveLeftEnd = "all_move_left_ed";
            public const string MoveRightStart = "all_move_right_st";
            public const string MoveRightLoop = "all_move_right_lp";
            public const string MoveRightEnd = "all_move_right_ed";
            public const string ShieldStart = "enemy_shield_st";  // 盾構え
            public const string ShieldLoop = "enemy_shield_lp";   // 盾構え
            public const string Attack = "enemy_shield_attack00"; // 攻撃？
            public const string Damage = "enemy_break01";
        }

        // ボスのAnimatorControllerのステート名
        public static class Boss
        {
            public const string Idle = "Idle";
            public const string MoveFrontLoop = "all_move_front_lp";
            public const string MoveFrontEnd = "all_move_front_ed";
            public const string MoveLeftStart = "all_move_left_st";
            public const string MoveLeftLoop = "all_move_left_lp";
            public const string MoveLeftEnd = "all_move_left_ed";
            public const string MoveRightStart = "all_move_right_st";
            public const string MoveRightLoop = "all_move_right_lp";
            public const string MoveRightEnd = "all_move_right_ed";
            public const string HoldStart = "enemy_rocket_hold_st"; // 銃構え
            public const string HoldLoop = "enemy_rocket_hold_lp";  // 銃構え
            public const string FireLoop = "enemy_rocket_fire";     // 銃攻撃
            public const string Reload = "enemy_rocket_reload";     // 銃リロード
            public const string BladeStart = "enemy_shield_st";        // 刀構え
            public const string BladeLoop = "enemy_shield_lp";         // 刀構え
            public const string BladeAttack = "enemy_shield_attack00"; // 刀攻撃
            public const string FunnelExpand = "boss_funnel_injection"; // ファンネル展開
            public const string FunnelFire = "boss_funnel_fire";        // ファンネル攻撃
            public const string FunnelDestroy = "boss_funnel_destroy";  // ファンネル破壊
            public const string Damage = "enemy_break00";
            public const string QteSwordSet = "boss_sword_set";            // 左手破壊、刀構え
            public const string QteSwordAttack_1 = "boss_sword_attack_01"; // 左手破壊、刀振り下ろす
            public const string QteSowrdRepel_1 = "boss_sword_repel_01";   // 左手破壊、刀で左手を吹き飛ばす？
            public const string QteSwordHold_2 = "boss_sword_hold_02";     // 鍔迫り合い、刀構え？
            public const string QteSwordAttack_2 = "boss_sword_attack_02"; // 鍔迫り合い、刀振り下ろす
            public const string QteSowrdRepel_2 = "boss_sword_repel_02";   // 鍔迫り合い、弾かれる
            public const string BossFinish = "boss_finish";
        }

        // ボスを含む敵のAnimatorControllreのパラメータ名の定数。
        public static class Param
        {
            public const string PlaySpeed = "PlaySpeed";
            public const string SpeedY = "SpeedY";
            public const string SpeedX = "SpeedX";
            public const string ApproachEnd = "FinishFirstMoveTrigger";
            public const string AttackSet = "AttackSetTrigger";
            public const string Attack = "AttackTrigger";
            public const string AttackEnd = "AttackEndTrigger";
            public const string BladeAttack = "BladeAttackTrigger00";
            public const string BladeAttackEnd = "BladeAttackEndTrigger00";
            public const string GetDamage = "GetDamageTrigger";
            public const string FunnelFire = "FunnelFireTrigger";
            public const string FunnelExpand = "FunnelInjectionTrigger";
            public const string FunnelDestroy = "FunnelDestroyTrigger";
            public const string Break = "BreakTrigger";
            public const string QteBladeAttackSet = "QteBladeAttackSetTrigger";
            public const string QteBladeAttack01 = "QteBladeAttackTrigger01";
            public const string QteBladeAttackClear01 = "QteBladeAttackClearTrigger01";
            public const string QteBladeAttack02 = "QteBladeAttackTrigger02";
            public const string QteBladeAttackClear02 = "QteBladeAttackClearTrigger02";
            public const string Finish = "FinishTrigger";
        }

        // AnimatorControllerのレイヤー
        public static class Layer
        {
            public const int BaseLayer = 0;
            public const int UpperBody = 1;
        }
    }
}
