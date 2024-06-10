using Enemy.Control.BT;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// どのようにキャラクターを制御するかを決める。
    /// </summary>
    public class BehaviorTree
    {
        private Sequence _approach;
        private Sequence _chase;
        private Sequence _attack;
        private Sequence _idle;
        private Sequence _escape;
        private Sequence _broken;
        private Sequence _hide;
        private Sequence _damaged;
        private BlackBoard _blackBoard;

        public BehaviorTree(Transform transform, EnemyParams enemyParams, BlackBoard blackBoard)
        {
            _approach = new Sequence(
                "ApproachSequence",
                new MoveToPlayer(Choice.Approach, transform, enemyParams, blackBoard),
                new LookAtPlayer(Choice.Approach, blackBoard),
                new WriteActionPlan(Choice.Approach, blackBoard)
                );

            _chase = new Sequence(
                "ChaseSequence", 
                new MoveToPlayer(Choice.Chase, transform, enemyParams, blackBoard),
                new LookAtPlayer(Choice.Chase, blackBoard),
                new WriteActionPlan(Choice.Chase, blackBoard)
                );

            _attack = new Sequence(
                "AttackSequence",
                new CheckAttackConditions(transform, enemyParams, blackBoard),
                new WriteActionPlan(Choice.Attack, blackBoard)
                );

            _escape = new Sequence(
                "EscapeSequence",
                new MoveVertical(enemyParams, blackBoard),
                new WriteActionPlan(Choice.Escape, blackBoard)
                );

            _broken = new Sequence(
                "BrokenSequence",
                new WriteActionPlan(Choice.Broken, blackBoard));

            _hide = new Sequence(
                "HideSequence",
                new WriteActionPlan(Choice.Hide, blackBoard));

            _damaged = new Sequence(
                "DamagedSequence",
                new WriteActionPlan(Choice.Damaged, blackBoard));

            _blackBoard = blackBoard;
        }

        /// <summary>
        /// ビヘイビアツリーを実行。
        /// どのように制御するかをBlackBoardクラスに書き込む。
        /// </summary>
        public void Run(Choice choice)
        {
            if (choice == Choice.Approach) _approach.Update();
            else if (choice == Choice.Chase) _chase.Update();
            else if (choice == Choice.Attack) _attack.Update();
            else if (choice == Choice.Escape) _escape.Update();
            else if (choice == Choice.Broken) _broken.Update();
            else if (choice == Choice.Hide) _hide.Update();
            else if (choice == Choice.Damaged) _damaged.Update();

            // ツリーを実行しても必ず行動として選択されるとは限らないので、
            // ノードの実行のたびに値を変化させるとバグる。
        }

        /// <summary>
        /// フレームを跨ぐ前に各ノードで書きこんだ内容を消す。
        /// </summary>
        public void ClearBlackBoardWritedValues()
        {
            _blackBoard.ActionPlans.Clear();
            _blackBoard.WarpPlans.Clear();
            _blackBoard.MovePlans.Clear();
            _blackBoard.LookPlans.Clear();
        }
    }
}