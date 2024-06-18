using System.Collections;
using System.Collections.Generic;
using Enemy.Extensions;
using UnityEngine;

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
        BreakLeftArm, // プレイヤーの左手破壊
        FirstQte,     // ボス1回目のQTE
        SecondQte,    // ボス2回目のQTE
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

            // ボス戦開始した状態は何もしない。
            if (!_blackBoard.IsBossStarted) return _order;

            // まずは登場する。
            if (!_blackBoard.IsAppearCompleted) { _order.Add(Choice.Appear); return _order; }

            // 終盤のQTEイベント中
            if (_blackBoard.IsQteEventStarted)
            {
                // 命令によって分岐
                switch (_blackBoard.OrderdQteEventStep)
                {
                    // プレイヤーの左手破壊
                    case FSM.QteEventState.Step.BreakLeftArm: _order.Add(Choice.BreakLeftArm); break;
                    // QTE1回目
                    case FSM.QteEventState.Step.FirstQte: _order.Add(Choice.FirstQte); break;
                    // QTE2回目
                    case FSM.QteEventState.Step.SecondQte: _order.Add(Choice.SecondQte); break;
                }

                return _order;
            }

            // ファンネル展開。
            if (_blackBoard.FunnelExpandTrigger) _order.Add(Choice.FunnelExpand);

            // 近接攻撃の範囲内かつ、タイミングが来ていた場合は攻撃する。
            if (_blackBoard.IsWithinMeleeRange && _blackBoard.NextMeleeAttackTime < Time.time)
            {
                _order.Add(Choice.BladeAttack);
            }
            // または、遠距離攻撃タイミングが来ていた場合は攻撃する。
            else if (_blackBoard.NextRangeAttackTime < Time.time)
            {
                _order.Add(Choice.RifleFire);
            }

            // ボス戦開始後、登場が完了した場合は、プレイヤーを追いかける。
            _order.Add(Choice.Chase);

            return _order;
        }
    }
}