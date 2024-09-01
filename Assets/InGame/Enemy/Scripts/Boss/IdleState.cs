using UnityEngine;
using System.Collections.Generic;

namespace Enemy.Boss
{
    /// <summary>
    /// どの行動を選択するかを決める。
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
            bool isFunnelExpand = Ref.BlackBoard.FunnelExpand.IsWaitingExecute();
            if (isFunnelExpand) { TryChangeState(StateKey.FunnelExpand); return; }

            // QTEイベントが始まった場合は遷移。
            bool isQteStarted = Ref.BlackBoard.IsQteStarted;
            if (isQteStarted) { TryChangeState(StateKey.QteEvent); return; }

            // 近接攻撃の範囲内かつ、タイミングが来ていた場合は攻撃。
            // マルチロックのテストをさせてあげるためにウロチョロしないでほしいので用に一旦コメントアウト。
            //bool isMeleeRange = Ref.BlackBoard.IsWithinMeleeRange;
            //bool isMelee = Ref.BlackBoard.MeleeAttack.IsWaitingExecute();
            //if (isMeleeRange && isMelee) { TryChangeState(StateKey.BladeAttack); return; }

            // または、遠距離攻撃タイミングが来ていた場合は攻撃。
            if (IsRangeAttackSelected()) { TryChangeState(StateKey.LauncherFire); return; }

            TryChangeState(StateKey.LaneChange);
        }

        // 遠距離攻撃を選択する。
        private bool IsRangeAttackSelected()
        {
            // 攻撃タイミングが来ているか？
            bool isTiming = Ref.BlackBoard.RangeAttack.IsWaitingExecute();

            if (!isTiming) return false;

            // プレイヤーの反対側のレーン。
            int pi = Ref.Field.CurrentRane.Value;
            int length = Ref.Field.LaneList.Count;
            int target = LaneChangeStep.GetOtherSideLane(pi, length);
            // 現在のレーンから時計回りと反時計回りで移動する場合の移動回数が少ない方を選択。
            int current = Ref.BlackBoard.CurrentLaneIndex;
            int clockwiseLength = LaneChangeStep.GetRest(target, current, length);
            int counterclockwiseLength = LaneChangeStep.GetRest(current, target, length);
            // プレイヤーが攻撃範囲のレーン内にいるか？
            const int Range = 1;
            bool isInRange = Mathf.Min(clockwiseLength, counterclockwiseLength) <= Range;

            return isInRange;
        }
    }
}