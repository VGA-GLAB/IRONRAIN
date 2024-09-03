using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel
{
    public class BattleState : State<StateKey>
    {
        // オフセットをホバリングさせる。
        private float _hovering;

        public BattleState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        protected RequiredRef Ref { get; private set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Battle;

            // ホバリングが揃っていると不自然なのでランダム性を持たせる。
            _hovering = Random.Range(-1.0f, 1.0f);
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            TraceMove();
            Look();
            Fire();
            Hovering();

            bool isDead = !Ref.BlackBoard.IsAlive;
            if (isDead) { TryChangeState(StateKey.Return); return; }
        }

        // ボスを追従するように移動。
        private void TraceMove()
        {
            Vector3 offset = (Vector3)Ref.BlackBoard.ExpandOffset;
            Vector3 dx = Ref.Body.Right * offset.x;
            Vector3 dy = Ref.Body.Up * offset.y;
            Vector3 dz = Ref.Body.Forward * offset.z;
            Vector3 bp = Ref.Boss.transform.position;
            Vector3 p = bp + dx + dy + dz;
            Vector3 dir = p - Ref.Body.Position;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            float spd = Ref.FunnelParams.MoveSpeed.Chase;
            Vector3 velo = dir.normalized * dt * spd;
            if (velo.sqrMagnitude <= dir.sqrMagnitude) Ref.Body.Move(velo);
            else Ref.Body.Warp(p);
        }

        // プレイヤーの方向、もしくはボスと同じ方向を向く。
        private void Look()
        {
            FireMode mode = Ref.FunnelParams.FireMode;
            Vector3 look;
            if (mode == FireMode.Player)
            {
                look = Ref.BlackBoard.PlayerDirection;
            }
            else
            {
                look = Ref.BossRotate.forward;
            }

            Ref.Body.LookForward(look);
        }

        // 射撃。
        private void Fire()
        {
            bool isDisabled = !Ref.BlackBoard.IsFireEnabled;
            if (isDisabled) return;

            Trigger attack = Ref.BlackBoard.Attack;
            if (!attack.IsWaitingExecute()) return;
            
            attack.Execute();

            IOwnerTime owner = Ref.Boss.BlackBoard;

            float ac = Ref.FunnelParams.Accuracy;
            float rx = Random.value * ac;
            float ry = Random.value * ac;
            float rz = Random.value * ac;
            FireMode mode = Ref.FunnelParams.FireMode;
            Vector3 bf;
            if (mode == FireMode.Player)
            {
                bf = Ref.BlackBoard.PlayerDirection;
            }
            else
            {
                bf = Ref.BossRotate.forward;
            }

            Vector3 forward = bf + new Vector3(rx, ry, rz);

            if (Ref.BulletPool.TryRent(BulletKey.Funnel, out Bullet bullet))
            {
                bullet.transform.position = Ref.Muzzle.position;
                bullet.Shoot(forward, owner);
            }
        }

        // オフセットを上下させてホバリング。
        private void Hovering()
        {
            float h = Mathf.Sin(_hovering);
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _hovering += dt;
            Ref.Body.OffsetWarp(Vector3.up * h);
        }
    }
}
