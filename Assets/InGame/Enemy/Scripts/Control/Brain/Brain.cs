using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 黒板に書き込まれた内容から次にとるべき行動を決定する。
    /// </summary>
    public class Brain : ILifeCycleHandler
    {
        private UtilityEvaluator _evaluator;
        private BehaviorTree _tree;

        public Brain(Transform transform, EnemyParams enemyParams, BlackBoard blackBoard)
        {
            _evaluator = new UtilityEvaluator();
            _tree = new BehaviorTree(transform, enemyParams, blackBoard);
        }

        public void UpdateEvent()
        {
            Choice[] eval = _evaluator.Evaluate();
            
            // 優先度順で全ての行動に対する制御を決める。
            // 後々、優先度の低い行動は省くような処理が入るかもしれない。
            for (int i = 0; i < eval.Length; i++)
            {
                _tree.Run(eval[i]);
            }
        }
    }
}