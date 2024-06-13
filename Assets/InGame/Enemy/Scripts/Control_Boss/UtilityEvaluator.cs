using System.Collections;
using System.Collections.Generic;
using Enemy.Extensions;

namespace Enemy.Control.Boss
{
    /// <summary>
    /// キャラクターが取りうる行動の選択肢
    /// </summary>
    public enum Choice
    {
        Idle,         // 何もしない
        Appear,       // ボス登場
        Chase,        // プレイヤーを追跡
        BladeAttack,  // 刀で攻撃
        RifleFire,    // ライフルで攻撃
        FunnelExpand, // ファンネルを展開
    }

    /// <summary>
    /// 情報をもとに何をすべきかを判定する。
    /// </summary>
    public class UtilityEvaluator
    {
        private BossParams _params;
        private BlackBoard _blackBoard;

        private List<Choice> _order;

        public UtilityEvaluator(BossParams bossParams, BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
            _params = bossParams;
            _order = new List<Choice>(EnumExtensions.Length<Choice>());
        }

        /// <summary>
        /// 行動を評価して優先度順にソートして返す。
        /// </summary>
        public IReadOnlyList<Choice> Evaluate()
        {
            _order.Clear();

            // ボス戦開始した状態
            if (_blackBoard.IsBossStarted)
            {
                // まずは登場する。
                if (!_blackBoard.IsAppearCompleted) { _order.Add(Choice.Appear); return _order; }

                // ファンネル展開。
                if (_blackBoard.FunnelExpandTrigger) _order.Add(Choice.FunnelExpand);

                // 次ここにFirstQTEとSecondQTEの処理

                // ボス戦開始後、登場が完了した場合は、プレイヤーを追いかける。
                _order.Add(Choice.Chase);
            }

            return _order;
        }
    }
}