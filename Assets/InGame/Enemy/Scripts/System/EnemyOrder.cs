using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 敵に命令するときにEnemyController側に渡す内容。
    /// </summary>
    public class EnemyOrder
    {
        public enum Type
        {
            None,
            PlayerDetect,     // プレイヤーを発見状態にさせる。
            Pause,            // ポーズ。
            Resume,           // ポーズ解除。
            Attack,           // 攻撃させる。
            QteTriggerEnter,  // QTEの判定に触れた(盾持ちの敵が自身に命令)。
            BossStart,        // ボス戦開始。
            FunnelExpand,     // ファンネル展開。
            FunnelLaserSight, // ファンネルのレーザーサイト表示。
            MoveToPlayerFront,     // プレイヤーの正面に移動。
            BreakLeftArm,          // プレイヤーの左腕破壊。
            QteCombatReady,        // 左腕破壊->鍔迫り合い。
            FirstQteCombatAction,  // 鍔迫り合い1回目。
            SecondQteCombatAction, // 鍔迫り合い2回目。
            PenetrateBoss,         // パイルバンカーでボスが貫かれる。
            QteStartTargeted,     // 自身を対象としてQTE開始。
            QteStartUntargeted,   // 自身以外を対象としてQTE開始。
            QteSuccessTargeted,   // 自身を対象としてQTE成功。
            QteSuccessUntargeted, // 自身以外を対象としてQTE成功。
            QteFailureTargeted,   // 自身を対象としてQTE失敗。
            QteFailureUntargeted, // 自身以外を対象としてQTE失敗。
        };

        /// <summary>
        /// 命令内容
        /// </summary>
        public Type OrderType;
        /// <summary>
        /// 位置の指定
        /// </summary>        
        public Vector3? Point;

        public void Clear()
        {
            OrderType = Type.None;
            Point = default;
        }
    }
}