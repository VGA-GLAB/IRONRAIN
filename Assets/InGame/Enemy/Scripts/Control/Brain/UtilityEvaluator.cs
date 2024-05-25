using Enemy.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// キャラクターが取りうる行動の選択肢
    /// </summary>
    public enum Choice
    {
        Idle,     // 何もしない
        Approach, // 生成~指定位置まで接近
        Chase,    // プレイヤーを追跡
        Attack,   // 攻撃
        Broken,   // 死亡
        Escape,   // 撤退
        Hide,     // 生成後、画面から隠す
    }

    /// <summary>
    /// 情報をもとに何をすべきかを判定する。
    /// </summary>
    public class UtilityEvaluator
    {
        private BlackBoard _blackBoard;
        private IApproach _approach;

        private List<Choice> _order;

        public UtilityEvaluator(BlackBoard blackBoard, IApproach approach)
        {
            _blackBoard = blackBoard;
            _approach = approach;
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

            // 接近検知範囲外の場合は画面から隠す
            if (!_blackBoard.IsPlayerDetected) 
            {
                _order.Add(Choice.Hide); 
                return _order; 
            }

            // 接近中の場合はインターフェースを実装したクラス側で制御するので何もしない。
            if (_approach != null && !_approach.IsCompleted()) { /*実装まだ*/ }

            // 接近が完了したフラグで判定。
            // 攻撃中にプレイヤーを追跡していると、離れることで接近の条件を満たすのを防ぐ。
            if (_approach == null && !_blackBoard.IsApproachCompleted)
            {
                _order.Add(Choice.Approach);
            }
            else
            {
                if (_blackBoard.LifeTime <= 0)
                {
                    // 時間が0以下の場合は撤退
                    _order.Add(Choice.Escape);
                }
                else
                {
                    // それ以外は移動しつつ攻撃
                    _order.Add(Choice.Chase);
                    _order.Add(Choice.Attack);
                }
            }

            return _order;
        }
    }
}
