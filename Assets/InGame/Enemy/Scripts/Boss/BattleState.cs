using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Funnel;

namespace Enemy.Boss
{
    /// <summary>
    /// 戦闘中の各行動をステートで管理するための基底クラス。
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
        /// ファンネルのレーザーサイトを表示
        /// </summary>
        protected void FunnelLaserSight()
        {
            bool isView = Ref.BlackBoard.IsFunnelLaserSight;
            if (isView)
            {
                foreach (FunnelController f in Ref.Funnels) f.LaserSight(true);
            }
        }

        /// <summary>
        /// 点Pに向けて移動。
        /// </summary>
        protected void MoveToPointP()
        {
            Vector3 p;

            Vector3 dir = Ref.BlackBoard.PointPDirection;
            float spd = Ref.BossParams.MoveSpeed.Chase;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            Vector3 mpf = dir * spd * dt;
            float dist = Ref.BlackBoard.PointPDistance;
            if (mpf.magnitude >= dist)
            {
                p = Ref.PointP.transform.position;
            }
            else
            {
                p = Ref.BlackBoard.Area.Point + mpf;
            }

            Ref.Body.Warp(p);
        }

        /// <summary>
        /// プレイヤーを向く。
        /// </summary>
        protected void LookAtPlayer()
        {
            BlackBoard bb = Ref.BlackBoard;
            float dt = bb.PausableDeltaTime;

            bb.PlayerLookElapsed += dt;

            // ボスの前方向に射撃するファンネルを機能させるために一定間隔。
            float duration = Ref.BossParams.LookDuration;
            if (bb.PlayerLookElapsed > duration)
            {
                bb.PlayerLookElapsed = 0;

                Vector3 pd = bb.PlayerDirection;
                pd.y = 0;
                bb.LookDirection = pd;
            }

            const float LookSpeed = 5.0f; // 適当。
            Vector3 a = Ref.Body.Forward;
            Vector3 b = bb.LookDirection;
            Vector3 look = Vector3.Lerp(a, b, dt * LookSpeed);

            Ref.Body.LookForward(look);
        }
    }
}