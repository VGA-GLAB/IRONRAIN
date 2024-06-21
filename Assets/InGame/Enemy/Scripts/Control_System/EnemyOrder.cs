namespace Enemy.Control
{
    /// <summary>
    /// 敵に命令するときにEnemyController側に渡す内容。
    /// </summary>
    public class EnemyOrder
    {
        public enum Type
        {
            None,
            PlayerDetect,    // プレイヤーを発見状態にさせる。
            Pause,           // ポーズ。
            Resume,          // ポーズ解除。
            Attack,          // 攻撃させる。
            QteTriggerEnter, // QTEの判定に触れた(盾持ちの敵が自身に命令)。
            BossStart,       // ボス戦開始。
            FunnelExpand,    // ファンネル展開。
            BreakLeftArm,    // プレイヤーの左腕破壊~。
            BossFirstQTE,    // ボス戦QTE1回目。
            BossSecondQTE,   // ボス戦QTE2回目。
            QteStartTargeted,   // 自身を対象としてQTE開始。
            QteStartUntargeted, // 自身以外を対象としてQTE開始。
            QteEndTargeted,     // 自身を対象としてQTE終了。
            QteEndUntargeted,   // 自身以外を対象としてQTE終了。
        };

        public Type OrderType;

        public void Clear()
        {
            OrderType = Type.None;
        }
    }
}