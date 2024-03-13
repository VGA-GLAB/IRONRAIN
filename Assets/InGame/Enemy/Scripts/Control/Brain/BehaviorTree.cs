using Enemy.Control.BT;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// どのようにキャラクターを制御するかを決める。
    /// </summary>
    public class BehaviorTree
    {
        private Sequence _chase;
        private Sequence _attack;
        private Sequence _idle;

        public BehaviorTree(Transform transform, EnemyParams enemyParams, BlackBoard blackBoard)
        {
            _chase = new Sequence(
                "ChaseSequence", 
                new MoveToPlayer(transform, blackBoard, enemyParams));
        }

        public void Run(Choice choice)
        {
            if (choice == Choice.Chase) _chase.Update();

            // AttackとIdleも同じく書く
        }
    }
}