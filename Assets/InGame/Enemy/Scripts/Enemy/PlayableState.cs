﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// ApproachStateと雑魚3種それぞれのBattleStateの基底クラス。
    /// 登場させてから死ぬまでに共通して使うメソッド群。
    /// </summary>
    public abstract class PlayableState : State<StateKey>
    {
        public PlayableState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        protected RequiredRef Ref { get; }

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
        /// 移動した方向ベクトルで下半身のアニメーションを制御。
        /// z軸を前方向として、ベクトルのx成分の正負で左右どちらに移動したかを判定する。
        /// </summary>
        protected void MoveAnimation(in Vector3 moveDir)
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
        /// 死亡もしくは撤退の場合は、アイドル状態を経由して退場するステートに遷移。
        /// </summary>
        protected bool ExitIfDeadOrTimeOver()
        {
            if (Ref.BlackBoard.Hp <= 0) { TryChangeState(StateKey.Broken); return true; }
            else if (Ref.BlackBoard.LifeTime <= 0) { TryChangeState(StateKey.Escape); return true; }

            return false;
        }
    }
}
