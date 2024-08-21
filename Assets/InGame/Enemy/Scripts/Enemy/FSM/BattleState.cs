using Enemy.Extensions;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 接近状態と雑魚3種類それぞれの戦闘状態の基底クラス。
    /// Stayで呼ぶ前提のメソッドのみを持ち、呼び出し自体は行わない。
    /// </summary>
    public class BattleState : State<StateKey>
    {
        public BattleState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        protected RequiredRef Ref { get; private set; }

        protected override void Enter() { }
        protected override void Exit() { }
        protected override void Stay() { }

        /// <summary>
        /// ダメージを受けた場合に音を再生。
        /// </summary>
        protected void PlayDamageSE()
        {
            string source = Ref.BlackBoard.DamageSource;
            string seName = "";
            if (source == Const.PlayerRifleWeaponName) seName = "SE_Damage_02";
            else if (source == Const.PlayerLauncherWeaponName) seName = "SE_Missile_Hit";
            else if (source == Const.PlayerMeleeWeaponName) seName = "SE_PileBunker_Hit";
            
            if (seName != "") AudioWrapper.PlaySE(seName);
        }

        /// <summary>
        /// 死亡もしくは撤退の場合は、アイドル状態を経由して退場するステートに遷移。
        /// </summary>
        protected bool BattleExit()
        {
            if (Ref.BlackBoard.Hp <= 0) { TryChangeState(StateKey.Broken); return true; }
            else if (Ref.BlackBoard.LifeTime <= 0) { TryChangeState(StateKey.Escape); return true; }

            return false;
        }

        /// <summary>
        /// スロットの位置へアニメーションしつつ移動。
        /// </summary>
        protected void MoveToSlot(float speed)
        {
            // 移動前後の位置を比較して左右どちらに移動したかを判定する。
            Vector3 before = Ref.Body.Position;
            MoveToSlot(MovementPerFrame(speed));
            LookAtPlayer();
            Vector3 after = Ref.Body.Position;

            MoveAnimation(after - before);
        }

        // スロットの位置へ向かうベクトルを返す。
        private Vector3 MovementPerFrame(float speed)
        {
            Vector3 v = VectorExtensions.Homing(
                Ref.BlackBoard.Area.Point,
                Ref.BlackBoard.Slot.Point,
                Ref.BlackBoard.SlotDirection,
                Ref.EnemyParams.Other.ApproachHomingPower
                );
            float dt = Ref.BlackBoard.PausableDeltaTime;
            return v * speed * dt;
        }

        // エリアの中心位置からスロット方向へ1フレームぶん移動した位置へワープさせる。
        // エリアの半径が小さすぎない限り、移動させても飛び出すことは無い。
        private void MoveToSlot(in Vector3 mpf)
        {
            Vector3 p;

            float sqrDist = Ref.BlackBoard.SlotSqrDistance;
            if (mpf.sqrMagnitude >= sqrDist)
            {
                p = Ref.BlackBoard.Slot.Point;
            }
            else
            {
                p = Ref.BlackBoard.Area.Point + mpf;
            }

            Ref.Body.Warp(p);
        }

        // プレイヤーを向かせる。
        private void LookAtPlayer()
        {
            Vector3 dir = Ref.BlackBoard.PlayerDirection;
            dir.y = 0;

            Ref.Body.LookForward(dir);
        }

        // 移動した方向ベクトルでアニメーションを制御。
        // z軸を前方向として、ベクトルのx成分の正負で左右どちらに移動したかを判定する。
        private void MoveAnimation(in Vector3 moveDir)
        {
            BodyAnimation anim = Ref.BodyAnimation;
            if (Mathf.Abs(moveDir.x) < Mathf.Epsilon)
            {
                anim.SetBool(BodyAnimationConst.Param.IsLeftMove, false);
                anim.SetBool(BodyAnimationConst.Param.IsRightMove, false);
            }
            else if (moveDir.x < 0)
            {
                anim.SetBool(BodyAnimationConst.Param.IsLeftMove, false);
                anim.SetBool(BodyAnimationConst.Param.IsRightMove, true);
            }
            else if (moveDir.x > 0)
            {
                anim.SetBool(BodyAnimationConst.Param.IsLeftMove, true);
                anim.SetBool(BodyAnimationConst.Param.IsRightMove, false);
            }
        }

        /// <summary>
        /// 通常は攻撃範囲にプレイヤーがいるかつ、次の攻撃タイミングが来ていた場合は攻撃。
        /// 手動攻撃の場合は攻撃命令を受けていれば攻撃。
        /// </summary>
        public bool IsAttack()
        {
            SpecialCondition condition = Ref.EnemyParams.SpecialCondition;
            if (condition == SpecialCondition.ManualAttack)
            {
                Trigger attack = Ref.BlackBoard.OrderedAttack;
                return attack.IsOrdered();
            }
            else
            {
                Trigger attack = Ref.BlackBoard.Attack;
                return attack.IsOrdered() && CheckAttackRange();
            }
        }

        /// <summary>
        /// 攻撃のアニメーション再生処理を呼んだタイミングで同時に呼ぶ。
        /// 黒板に攻撃したことを書き込む。
        /// </summary>
        public void AttackTrigger()
        {
            Ref.BlackBoard.OrderedAttack.Execute();
            Ref.BlackBoard.Attack.Execute();
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
    }
}
