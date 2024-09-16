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

        protected override void OnEnter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Idle;
        }

        protected override void OnExit()
        {
        }

        protected override void OnStay()
        {
            Hovering();

            if (IsFunnelExpand()) { FunnelExpand(); return; }

            if (IsQteStarted()) { QTE(); return; }

            if (IsPauseOnPlayerForward()) { PauseOnPlayerForward(); return; }

            bool isMelee = IsMeleeAttackSelected();
            bool isRange = IsRangeAttackSelected();
            if (isMelee && isRange)
            {
                if (IsMeleeAttackRandomSelected()) MeleeAttack();
                else RangeFire();
            }
            else if (isMelee) MeleeAttack();
            else if (isRange) RangeFire();
            else LaneChange();

            // 行動一覧。
            void MeleeAttack() => TryChangeState(StateKey.BladeAttack);
            void RangeFire() => TryChangeState(StateKey.LauncherFire);
            void LaneChange() => TryChangeState(StateKey.LaneChange);
            void QTE() => TryChangeState(StateKey.QteEvent);
            void PauseOnPlayerForward() => TryChangeState(StateKey.PauseOnPlayerForward);
            void FunnelExpand() => TryChangeState(StateKey.FunnelExpand);
        }

        // ファンネル展開。
        private bool IsFunnelExpand()
        {
            return Ref.BlackBoard.FunnelExpand.IsWaitingExecute();
        }

        // QTEが開始された。
        private bool IsQteStarted()
        {
            return Ref.BlackBoard.IsQteStarted;
        }

        // マルチロックオンするため、プレイヤーの正面に移動。
        private bool IsPauseOnPlayerForward()
        {
            return Ref.BlackBoard.PauseOnPlayerForward.IsWaitingExecute();
        }

        // 近接攻撃を選択する。
        private bool IsMeleeAttackSelected()
        {
            bool isTiming = Ref.BlackBoard.MeleeAttack.IsWaitingExecute();
            if (!isTiming) return false;

            // 攻撃を行える範囲。
            const int Range = 1;
            return IsInRange(Range);
        }

        // 遠距離攻撃を選択する。
        private bool IsRangeAttackSelected()
        {
            bool isTiming = Ref.BlackBoard.RangeAttack.IsWaitingExecute();
            if (!isTiming) return false;

            // 攻撃を行える範囲。
            const int Range = 1;
            return IsInRange(Range);
        }

        // プレイヤーの正面のレーンを基準として、レーンの左右のズレで判定。
        private bool IsInRange(int range)
        {
            return Ref.Field.GetMinMoveCount() <= range;
        }

        // 遠距離攻撃と近接攻撃どちらも可能な場合、確率で近接攻撃を選ぶ。
        private bool IsMeleeAttackRandomSelected()
        {
            // 近接攻撃を選択する割合。
            const float MeleeRatio = 0.4f;

            return Random.value <= MeleeRatio;
        }
    }
}