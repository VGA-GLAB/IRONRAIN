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
        private AgentScript _agentScript;

        public IdleState(StateRequiredRef requiredRef) : base(requiredRef)
        {
            _agentScript = requiredRef.AgentScript;
        }

        protected override void Enter()
        {
            _blackBoard.CurrentState = StateKey.Idle;

            if (_agentScript != null) _agentScript.EnemyGenerate();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            PlayDamageSE();
            FunnelExpand();
            FunnelLaserSight();
            MoveToPointP();
            LookAtPlayer();

            // QTEイベントが始まった場合は遷移。
            if (_blackBoard.IsQteEventStarted)
            { 
                TryChangeState(StateKey.QteEvent); 
            }
            // 近接攻撃の範囲内かつ、タイミングが来ていた場合は攻撃。
            else if (_blackBoard.IsWithinMeleeRange && _blackBoard.NextMeleeAttackTime < Time.time)
            {
                TryChangeState(StateKey.BladeAttack);
            }
            // または、遠距離攻撃タイミングが来ていた場合は攻撃。
            else if (_blackBoard.NextRangeAttackTime < Time.time)
            {
                TryChangeState(StateKey.LauncherFire);
            }
        }
    }
}