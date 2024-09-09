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

        /// <summary>
        /// プレイヤーの方に向ける。
        /// </summary>
        protected void TurnToPlayer(bool isReset = false)
        {
            // ステートの切り替わりなどで呼び出しの間隔が開いた場合は、基準を現在の前向きにリセット。
            if (isReset)
            {
                Ref.BlackBoard.TurnForward = Ref.Body.Forward;
            }

            Vector3 look = Ref.BlackBoard.TurnForward;
            Ref.Body.LookForward(look);

            // 振り向き速度
            const float Speed = 10.0f;
        
            // 照準をしながらプレイヤーを向く。
            Vector3 pd = Ref.BlackBoard.PlayerDirection;
            pd.y = 0;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            look += pd * dt * Speed;
            Ref.BlackBoard.TurnForward = look.normalized;
        }
    }
}