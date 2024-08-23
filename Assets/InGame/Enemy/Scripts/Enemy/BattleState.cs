using Enemy.Extensions;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 雑魚3種それぞれのBattleStateの基底クラス。
    /// </summary>
    public abstract class BattleState : PlayableState
    {
        public BattleState(RequiredRef requiredRef) : base(requiredRef)
        {
        }

        protected override void Enter() { }
        protected override void Exit() { }      
        protected sealed override void Stay() 
        {
            PlayDamageSE();
            if (ExitIfDeadOrTimeOver()) return;
            WarpToSlot();

            StayIfBattle();
        }

        /// <summary>
        /// ダメージ音、死亡または時間切れで遷移、アニメーション付きの移動。
        /// これらが処理された後、装備している武器毎に動きが違う。
        /// </summary>
        protected abstract void StayIfBattle();

        /// <summary>
        /// スロットの位置に座標を変更。
        /// </summary>
        protected void WarpToSlot()
        {
            Vector3 before = Ref.Body.Position;

            Vector3 sp = Ref.BlackBoard.Slot.Point;
            Ref.Body.Warp(sp);

            Vector3 dir = Ref.BlackBoard.PlayerDirection;
            dir.y = 0;
            Ref.Body.LookForward(dir);

            Vector3 after = Ref.Body.Position;

            // 移動前後の位置を比較して左右どちらに移動したかを判定する。
            MoveAnimation(after - before);
        }

        // スロットの位置へ向かうベクトルを返す。
        private Vector3 MovementPerFrame()
        {
            Vector3 v = VectorExtensions.Homing(
                Ref.BlackBoard.Area.Point,
                Ref.BlackBoard.Slot.Point,
                Ref.BlackBoard.SlotDirection,
                Ref.EnemyParams.Other.ApproachHomingPower
                );
            float dt = Ref.BlackBoard.PausableDeltaTime;
            return v * dt;
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
