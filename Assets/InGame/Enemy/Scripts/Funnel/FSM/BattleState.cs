using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel
{
    public class BattleState : State<StateKey>
    {
        public BattleState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        protected RequiredRef Ref { get; private set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Battle;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            Vector3 offset = (Vector3)Ref.BlackBoard.ExpandOffset;
            Vector3 dx = Ref.Body.Right * offset.x;
            Vector3 dy = Ref.Body.Up * offset.y;
            Vector3 dz = Ref.Body.Forward * offset.z;
            Vector3 bp = Ref.Boss.transform.position;
            Vector3 p = bp + dx + dy + dz;
            Vector3 dir = p - Ref.Body.Position;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            float spd = Ref.FunnelParams.MoveSpeed;
            Vector3 velo = dir.normalized * dt * spd;
            if (velo.sqrMagnitude <= dir.sqrMagnitude) Ref.Body.Move(velo);
            else Ref.Body.Warp(p);

            ExpandMode mode = Ref.FunnelParams.ExpandMode;
            if (mode == ExpandMode.Right || mode == ExpandMode.Left)
            {
                Vector3 f = Ref.BlackBoard.PlayerDirection;
                Ref.Body.LookForward(f);
            }
            else
            {
                Vector3 f = Ref.BossRotate.forward;
                Ref.Body.LookForward(f);
            }

            if (Ref.BlackBoard.Fire.IsWaitingExecute())
            {
                Ref.BlackBoard.Fire.Execute();

                IOwnerTime owner = Ref.Boss.BlackBoard;
                Vector3 muzzle = Ref.Muzzle.position;

                float ac = Ref.FunnelParams.Accuracy;
                float rx = Random.value * ac;
                float ry = Random.value * ac;
                float rz = Random.value * ac;
                Vector3 bf = Ref.BossRotate.forward;
                Vector3 forward = bf + new Vector3(rx, ry, rz);

                BulletPool.Fire(owner, BulletKey.Funnel, muzzle, forward);
            }

            bool isDead = !Ref.BlackBoard.IsAlive;
            if (isDead) { TryChangeState(StateKey.Return); return; }
        }
    }
}
