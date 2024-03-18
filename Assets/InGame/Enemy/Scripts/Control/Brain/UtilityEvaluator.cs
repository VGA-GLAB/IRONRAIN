using Enemy.Extensions;
using System;
using System.Collections.Generic;

namespace Enemy.Control
{
    /// <summary>
    /// キャラクターが取りうる行動の選択肢
    /// </summary>
    public enum Choice
    {
        Idle,   // 何もしない
        Chase,  // プレイヤーを追跡
        Attack, // 攻撃
        Broken,  // 死亡
    }

    /// <summary>
    /// 情報をもとに何をすべきかを判定する。
    /// </summary>
    public class UtilityEvaluator
    {
        private BlackBoard _blackBoard;

        private List<Choice> _order;
        
        public UtilityEvaluator(BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
            _order = new List<Choice>(EnumExtensions.Length<Choice>());
        }
        
        /// <summary>
        /// 行動を評価して優先度順にソートして返す。
        /// </summary>
        public IReadOnlyList<Choice> Evaluate()
        {
            _order.Clear();

            // 死亡している場合は死亡が最優先
            if (_blackBoard.Hp <= 0) _order.Add(Choice.Broken);

            // プレイヤーが移動していたら移動したい
            _order.Add(Choice.Chase);
            // 攻撃のタイミングなら攻撃したい
            _order.Add(Choice.Attack);
            // どちらも満たしていない場合は何もしない
            _order.Add(Choice.Idle);

            return _order;
        }
    }
}
