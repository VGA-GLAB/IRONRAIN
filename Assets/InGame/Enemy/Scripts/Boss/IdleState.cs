using UnityEngine;
using System.Collections.Generic;

namespace Enemy.Boss
{
    /// <summary>
    /// 登場後、戦闘するステート。
    /// 攻撃する際は刀とロケットランチャー、それぞれのステートが担当する。
    /// </summary>
    public class IdleState : BattleState
    {
        public IdleState(RequiredRef requiredRef) : base(requiredRef) { }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Idle;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            PlayDamageSE();
            FunnelLaserSight();
            Hovering();

            // ファンネル展開。
            // 移動のテストするので一旦米ッとアウト。
            //bool isFunnelExpand = Ref.BlackBoard.FunnelExpand.IsWaitingExecute();
            //if (isFunnelExpand) { TryChangeState(StateKey.FunnelExpand); return; }

            // QTEイベントが始まった場合は遷移。
            bool isQteStarted = Ref.BlackBoard.IsQteStarted;
            if (isQteStarted) { TryChangeState(StateKey.QteEvent); return; }

            // 近接攻撃の範囲内かつ、タイミングが来ていた場合は攻撃。
            // マルチロックのテストをさせてあげるためにウロチョロしないでほしいので用に一旦コメントアウト。
            //bool isMeleeRange = Ref.BlackBoard.IsWithinMeleeRange;
            //bool isMelee = Ref.BlackBoard.MeleeAttack.IsWaitingExecute();
            //if (isMeleeRange && isMelee) { TryChangeState(StateKey.BladeAttack); return; }

            // または、遠距離攻撃タイミングが来ていた場合は攻撃。
            //bool isRange = Ref.BlackBoard.RangeAttack.IsWaitingExecute();
            //if (isRange) { TryChangeState(StateKey.LauncherFire); return; }

            //TryChangeState(StateKey.LaneChange);
        }
    }
}