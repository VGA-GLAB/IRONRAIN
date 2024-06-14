using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ビヘイビアツリーのSequenceクラス、各ノードの基底クラスのNodeクラスはEnemyのものを流用する。
using EnemyBT = Enemy.Control.BT;
using BossBT = Enemy.Control.Boss.BT;

namespace Enemy.Control.Boss
{
    /// <summary>
    /// どのようにキャラクターを制御するかを決める。
    /// </summary>
    public class BehaviorTree
    {
        private EnemyBT.Sequence _appear;
        private EnemyBT.Sequence _chase;
        private EnemyBT.Sequence _bladeAttack;
        private EnemyBT.Sequence _rifleFire;
        private EnemyBT.Sequence _funnelExpand;
        private EnemyBT.Sequence _breakLeftArm;
        private EnemyBT.Sequence _firstQte;
        private EnemyBT.Sequence _secondQte;

        private BlackBoard _blackBoard;

        public BehaviorTree(Transform transform, BossParams bossParams, BlackBoard blackBoard)
        {
            _appear = new EnemyBT.Sequence(
                "AppearSeq",
                new BossBT.WriteActionPlan(Choice.Appear, blackBoard)
                );

            _chase = new EnemyBT.Sequence(
                "ChaseSeq",
                new BossBT.MoveToPointP(transform, bossParams, blackBoard),
                new BossBT.WriteActionPlan(Choice.Chase, blackBoard)
                );

            _bladeAttack = new EnemyBT.Sequence(
                "BladeAttackSeq",
                new BossBT.WriteActionPlan(Choice.BladeAttack, blackBoard)
                );

            _rifleFire = new EnemyBT.Sequence(
                "RifleFireSeq",
                new BossBT.WriteActionPlan(Choice.RifleFire, blackBoard)
                );

            _funnelExpand = new EnemyBT.Sequence(
                "FunnelExpandSeq",
                new BossBT.WriteActionPlan(Choice.FunnelExpand, blackBoard)
                );

            _breakLeftArm = new EnemyBT.Sequence(
                "BreakLeftArmSeq",
                new BossBT.WriteActionPlan(Choice.BreakLeftArm, blackBoard)
                );

            _firstQte = new EnemyBT.Sequence(
                "FirstQteSeq",
                new BossBT.WriteActionPlan(Choice.FirstQte, blackBoard)
                );

            _secondQte = new EnemyBT.Sequence(
                "SecondQteSeq",
                new BossBT.WriteActionPlan(Choice.SecondQte, blackBoard)
                );

            _blackBoard = blackBoard;
        }

        /// <summary>
        /// ビヘイビアツリーを実行。
        /// どのように制御するかをBlackBoardクラスに書き込む。
        /// </summary>
        public void Run(Choice choice)
        {
            if (choice == Choice.Appear) _appear.Update();
            else if (choice == Choice.Chase) _chase.Update();
            else if (choice == Choice.BladeAttack) _bladeAttack.Update();
            else if (choice == Choice.RifleFire) _rifleFire.Update();
            else if (choice == Choice.FunnelExpand) _funnelExpand.Update();
            else if (choice == Choice.BreakLeftArm) _breakLeftArm.Update();
            else if (choice == Choice.FirstQte) _firstQte.Update();
            else if (choice == Choice.SecondQte) _secondQte.Update();
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
