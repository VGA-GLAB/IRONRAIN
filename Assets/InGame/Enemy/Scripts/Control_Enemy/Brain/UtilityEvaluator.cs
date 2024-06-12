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
        Damaged,  // ダメージを受ける
    }

    /// <summary>
    /// 情報をもとに何をすべきかを判定する。
    /// </summary>
    public class UtilityEvaluator
    {
        private EnemyParams _params;
        private BlackBoard _blackBoard;

        private List<Choice> _order;

        public UtilityEvaluator(EnemyParams enemyParams, BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
            _params = enemyParams;
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
            if (!_blackBoard.IsPlayerDetected) { _order.Add(Choice.Hide); return _order; }

            // 接近が完了したフラグで判定。
            // 攻撃中にプレイヤーを追跡していると、離れることで接近の条件を満たすのを防ぐ。
            if (!_blackBoard.IsApproachCompleted)
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
                else if (_blackBoard.CurrentFrameDamage > 0)
                {
                    // ダメージを受けた場合は怯む
                    _order.Add(Choice.Damaged);
                }
                else if (_params.Other.IsTutorial)
                {
                    _order.Add(Choice.Chase);

                    // チュートリアル用の敵の場合は、外部から攻撃処理を呼び出すことで攻撃する。                    
                    if (_blackBoard.OrderedAttackTrigger) _order.Add(Choice.Attack);
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
