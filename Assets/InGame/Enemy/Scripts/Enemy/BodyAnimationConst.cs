namespace Enemy
{
    public partial class BodyAnimationConst
    {
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
            public const string BladeStart = "enemy_shield_st";        // 刀構え
            public const string BladeLoop = "enemy_shield_lp";         // 刀構え
            public const string BladeAttack = "enemy_shield_attack00"; // 刀攻撃
            public const string FunnelExpand = "boss_funnel_injection"; // ファンネル展開
            public const string FunnelFire = "boss_funnel_fire";        // ファンネル攻撃
            public const string FunnelDestroy = "boss_funnel_destroy";  // ファンネル破壊
            public const string Damage = "enemy_break00";
            public const string QteSwordSet = "boss_sword_set";            // 左手破壊、刀構え
            public const string QteSwordAttack_1 = "boss_sword_attack_01"; // 左手破壊、刀振り下ろす
            public const string QteSowrdRepel_1 = "boss_sword_repel_01";   // 1回目QTE、刀構え直し
            public const string QteSwordAttack_2 = "boss_sword_attack_02"; // 1回目QTE、刀振り下ろす
            public const string QteSowrdRepel_2 = "boss_sword_repel_02";   // 2回目QTE、刀構え直し
            public const string BossFinish = "boss_finish";
        }

        // ボスを含む敵のAnimatorControllreのパラメータ名の定数。
        public static class Param
        {
            // この値をAnimatorのステートのSpeedに乗算した値が最終的なアニメーション再生速度。
            public const string PlaySpeed = "PlaySpeed";

            // アイドルのBlendTreeの制御に使われており、基本値は1。
            public const string SpeedY = "SpeedY";

            // プレイヤーを検知~接近が完了したタイミングをトリガー。
            public const string ApproachEnd = "FinishFirstMoveTrigger";

            // trueでアイドルから右に移動、falseで右に移動からアイドルに遷移。
            public const string IsRightMove = "IsRightMove";

            // trueでアイドルから左に移動、falseで左に移動からアイドルに遷移。
            public const string IsLeftMove = "IsLeftMove";

            // 武器を構える。
            public const string AttackSet = "AttackSetTrigger";

            // 武器で攻撃する。
            public const string Attack = "AttackTrigger";

            // 武器を下げてアイドル状態に戻る。
            public const string AttackEnd = "AttackEndTrigger";

            // 刀で攻撃する。現状ボス専用。
            public const string BladeAttack = "BladeAttackTrigger00";

            // ダメージを受ける。
            public const string GetDamage = "GetDamageTrigger";

            // ファンネル攻撃。
            public const string FunnelFire = "FunnelFireTrigger";

            // ファンネル展開。
            public const string FunnelExpand = "FunnelInjectionTrigger";

            // ファンネル破壊。
            public const string FunnelDestroy = "FunnelDestroyTrigger";

            // プレイヤーがQTE成功。盾持ちはGetDamageTriggerではなくこちらを使う。
            public const string Break = "BreakTrigger";

            // ボス戦のQTE、左腕を破壊する攻撃を構える。
            public const string QteBladeAttackSet = "QteBladeAttackSetTrigger";

            // プレイヤーの左手を破壊する攻撃を行う。
            public const string QteBladeAttack01 = "QteBladeAttackTrigger01";

            // ボス戦のQTE演出、左腕を破壊する攻撃を行った後、刀を構え直す。
            public const string QteBladeAttackClear01 = "QteBladeAttackClearTrigger01";

            // 鍔迫り合いになる。
            public const string QteBladeAttack02 = "QteBladeAttackTrigger02";

            // ボス戦のQTEの1回目、鍔迫り合いで弾かれて再度プレイヤーに突っ込む。
            public const string QteBladeAttackClear02 = "QteBladeAttackClearTrigger02";

            // ボス戦QTEの2回目、ボスが死ぬ。
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
