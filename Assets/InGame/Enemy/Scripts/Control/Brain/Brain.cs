using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 黒板に書き込まれた内容から次にとるべき行動を決定する。
    /// </summary>
    public class Brain : LifeCycle
    {
        private UtilityEvaluator _evaluator;
        private BehaviorTree _tree;
        private BlackBoard _blackBoard;

        public Brain(Transform transform, Transform rotate, EnemyParams enemyParams, BlackBoard blackBoard,
            IApproach approach)
        {
            _evaluator = new UtilityEvaluator(blackBoard, approach);
            _tree = new BehaviorTree(transform, rotate, enemyParams, blackBoard);
            _blackBoard = blackBoard;
        }

        public override Result UpdateEvent()
        {
            // プレイヤーの入力を受け取って何かしらの処理
            while (_blackBoard.PlayerInput.TryDequeue(out PlayerInputMessage msg))
            {
                // 処理ｺｺ
            }

            // 優先度順で全ての行動に対する制御を決める。
            // 後々、優先度の低い行動は省くような処理が入るかもしれない。
            IReadOnlyList<Choice> eval = _evaluator.Evaluate();
            for (int i = 0; i < eval.Count; i++)
            {
                _tree.Run(eval[i]);
            }

            return Result.Running;
        }

        public override Result LateUpdateEvent()
        {
            _tree.Clear();

            return Result.Running;
        }
    }
}