using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 雑魚3種類とも、攻撃周りの処理は共通なので、必要な処理を抜き出した。
    /// 構えと発射をそれぞれ担当するEnemyActionStepが、このクラスを継承する想定。
    /// </summary>
    public class EnemyAttackActionStep : EnemyActionStep
    {
        public EnemyAttackActionStep(RequiredRef requiredRef, params EnemyActionStep[] next) : base(requiredRef, next)
        {
        }

        /// <summary>
        /// 通常は攻撃範囲にプレイヤーがいるかつ、次の攻撃タイミングが来ていた場合は攻撃。
        /// 手動攻撃の場合は攻撃命令を受けていれば攻撃。
        /// </summary>
        protected bool IsAttack()
        {
            SpecialCondition condition = Ref.EnemyParams.SpecialCondition;
            if (condition == SpecialCondition.ManualAttack)
            {
                Trigger attack = Ref.BlackBoard.OrderedAttack;
                return attack.IsWaitingExecute();
            }
            else
            {
                Trigger attack = Ref.BlackBoard.Attack;
                return attack.IsWaitingExecute() && CheckAttackRange();
            }
        }

        // プレイヤーが攻撃範囲内にいるかチェック。
        private bool CheckAttackRange()
        {
            foreach (Collider c in Ref.BlackBoard.FovStay)
            {
                if (c.CompareTag(Const.PlayerTag)) return true;
            }

            return false;
        }

        /// <summary>
        /// 攻撃のアニメーション再生処理を呼んだタイミングで同時に呼ぶ。
        /// 黒板に攻撃したことを書き込む。
        /// </summary>
        protected void AttackTrigger()
        {
            Ref.BlackBoard.OrderedAttack.Execute();
            Ref.BlackBoard.Attack.Execute();
        }
    }
}
