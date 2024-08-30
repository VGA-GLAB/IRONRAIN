using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.NPC
{
    public class ApproachState : State<StateKey>
    {
        public ApproachState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Approach;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            float dist = Ref.BlackBoard.TargetSqrDistance;
            float DefeatDist = Ref.NpcParams.DefeatSqrDistance;
            if (dist < DefeatDist) { TryChangeState(StateKey.Action); return; }

            // プレイヤーより前に出た状態になるまで直進させる。
            // そうしないとプレイヤーを真後ろから通り抜けて敵に向かってしまう。
            const float Offset = 5.0f;
            float z = Ref.Body.Position.z;
            float pz = Ref.Player.position.z;
            bool isOver = z > pz + Offset;

            Vector3 dir;
            if (isOver) dir = Ref.BlackBoard.TargetDirection;
            else dir = Ref.Body.Forward;

            float spd = Ref.NpcParams.MoveSpeed;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            Vector3 velo = dir * dt * spd;
            Ref.Body.Move(velo);
        }
    }
}