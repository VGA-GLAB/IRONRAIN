using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Funnel;

namespace Enemy.Boss
{
    /// <summary>
    /// 戦闘中の各行動をステートで管理するための基底クラス。
    /// </summary>
    public class BattleState : State<StateKey>
    {
        public BattleState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        protected RequiredRef Ref { get; private set; }

        protected sealed override void Enter()
        {
            PlayDamageSE();
            FunnelLaserSight();
            OnEnter();
        }

        protected sealed override void Exit()
        {
            PlayDamageSE();
            FunnelLaserSight();
            OnExit();
        }

        protected sealed override void Stay()
        {
            PlayDamageSE();
            FunnelLaserSight();
            OnStay();
        }

        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnStay() { }

        // ダメージを受けた場合に音を再生。
        private void PlayDamageSE()
        {
            string source = Ref.BlackBoard.DamageSource;
            Vector3 p = Ref.Body.Position;
            DamageSE.Play(p, source);
        }

        // ファンネルのレーザーサイトを表示
        private void FunnelLaserSight()
        {
            bool isView = Ref.BlackBoard.IsFunnelLaserSight;
            if (isView)
            {
                foreach (FunnelController f in Ref.Funnels) f.LaserSight(true);
            }
        }

        /// <summary>
        /// オフセットの位置を上下させ、その場でホバリングさせる。
        /// </summary>
        protected void Hovering()
        {
            float h = Mathf.Sin(Ref.BlackBoard.Hovering);
            float dt = Ref.BlackBoard.PausableDeltaTime;
            Ref.BlackBoard.Hovering += dt;
            Ref.Body.OffsetWarp(Vector3.up * h);
        }
    }
}