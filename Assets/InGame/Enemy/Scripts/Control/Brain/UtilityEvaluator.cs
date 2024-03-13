using Enemy.Extensions;
using System;

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
    }

    /// <summary>
    /// 情報をもとに何をすべきかを判定する。
    /// </summary>
    public class UtilityEvaluator
    {
        private Choice[] _order;
        
        public UtilityEvaluator()
        {
            _order = new Choice[EnumExtensions.Length<Choice>()];
        }
        
        /// <summary>
        /// 行動を評価して優先度順にソートして返す。
        /// </summary>
        public Choice[] Evaluate()
        {
            Array.Clear(_order, 0, _order.Length);

            // 本来はここに優先度付け処理
            //  攻撃のタイミングなら攻撃したい
            //  プレイヤーが移動していたら移動したい
            //  どちらも満たしていない場合は何もしない
            _order[0] = Choice.Chase;
            _order[1] = Choice.Attack;
            _order[2] = Choice.Idle;

            return _order;
        }
    }
}
