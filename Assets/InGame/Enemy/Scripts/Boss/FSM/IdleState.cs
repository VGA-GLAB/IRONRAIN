using UnityEngine;
using System.Collections.Generic;

namespace Enemy.Boss.FSM
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

            // 登場後、戦闘開始と同時にレーダーマップに表示。
            AgentScript agent = Ref.AgentScript;
            if (agent != null) agent.EnemyGenerate();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            PlayDamageSE();
            FunnelLaserSight();
            MoveToPointP();
            LookAtPlayer();

            // ファンネル展開。
            bool isFunnelExpand = Ref.BlackBoard.FunnelExpand == Trigger.Ordered;
            if (isFunnelExpand) { TryChangeState(StateKey.FunnelExpand); return; }

            // QTEイベントが始まった場合は遷移。
            bool isQteStarted = Ref.BlackBoard.IsQteStarted;
            if (isQteStarted) { TryChangeState(StateKey.QteEvent); return; }

            // 近接攻撃の範囲内かつ、タイミングが来ていた場合は攻撃。
            bool isMeleeRange = Ref.BlackBoard.IsWithinMeleeRange;
            bool isMelee = Ref.BlackBoard.MeleeAttack == Trigger.Ordered;
            if (isMeleeRange && isMelee) { TryChangeState(StateKey.BladeAttack); return; }

            // または、遠距離攻撃タイミングが来ていた場合は攻撃。
            bool isRange = Ref.BlackBoard.RangeAttack == Trigger.Ordered;
            if (isRange) { TryChangeState(StateKey.LauncherFire); return; }
        }
    }
}