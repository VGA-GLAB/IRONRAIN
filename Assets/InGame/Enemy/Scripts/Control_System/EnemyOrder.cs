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
            PlayerDetect, // プレイヤーを発見状態にさせる。
            Pause,        // ポーズ。
            Resume,       // ポーズ解除。
            Attack,       // 攻撃させる。
            QteTrigger,   // QTE開始(盾持ちの敵が自身に命令)。
            BossStart,    // ボス戦開始。
            FunnelExpand, // ファンネル展開。
        };

        public Type OrderType;

        public void Clear()
        {
            OrderType = Type.None;
        }
    }
}