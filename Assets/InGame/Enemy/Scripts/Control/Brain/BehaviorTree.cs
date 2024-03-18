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
        private BlackBoard _blackBoard;

        public BehaviorTree(Transform transform, Transform rotate, EnemyParams enemyParams, BlackBoard blackBoard)
        {
            _chase = new Sequence(
                "ChaseSequence", 
                new MoveToPlayer(Choice.Chase, transform, rotate, blackBoard, enemyParams),
                new LookAtPlayer(Choice.Chase, blackBoard),
                new WriteActionPlan(Choice.Chase, blackBoard)
                );

            _attack = new Sequence(
                "AttackSequence",
                new CheckAttackConditions(transform, rotate, blackBoard, enemyParams),
                new WriteActionPlan(Choice.Attack, blackBoard)
                );

            _blackBoard = blackBoard;
        }

        /// <summary>
        /// ビヘイビアツリーを実行。
        /// どのように制御するかをBlackBoardクラスに書き込む。
        /// </summary>
        public void Run(Choice choice)
        {
            if (choice == Choice.Chase) _chase.Update();
            if (choice == Choice.Attack) _attack.Update();

            // NOTE:ツリーを実行しても必ず行動として選択されるとは限らないのでノードの実行のたびに値を変化させるとバグる。
            //      Updateで更新しているのなら、LateUpdateで諸々を消す？
        }
    }
}