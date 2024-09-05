using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel
{
    public class ExpandState : State<StateKey>
    {
        // Lerpで動かす。
        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;

        public ExpandState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        protected RequiredRef Ref { get; private set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Expand;

            AgentScript agent = Ref.AgentScript;
            if (agent != null) agent.EnemyGenerate();

            Ref.Effector.TrailEnable(true);

            // ファンネルが飛んでいる音(ループしなくて良い？)
            Vector3 p = Ref.Body.Position;
            int index = AudioWrapper.PlaySE(p, "SE_Funnel_Fly");
            Ref.BlackBoard.FlySeIndex = index;

            // ボスの位置に座標を変更し、そこから展開する。
            Vector3 boss = Ref.Boss.transform.position;
            Ref.Body.Warp(boss);

            // 展開時、ボスは立ち止まっている想定なので、Enterで展開位置を固定しても違和感ない？
            _start = Ref.Transform.position;
            Vector3 offset = (Vector3)Ref.BlackBoard.ExpandOffset;
            Vector3 ox = Ref.Body.Right * offset.x;
            Vector3 oy = Ref.Body.Up * offset.y;
            Vector3 oz = Ref.Body.Forward * offset.z;
            Vector3 bp = Ref.Boss.transform.position;
            _end = bp + ox + oy + oz;
            _lerp = 0;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            MoveToOffsetedPoint();

            if (IsMoveCompleted()) TryChangeState(StateKey.Battle);
        }

        // Lerpで移動。
        private void MoveToOffsetedPoint()
        {
            Vector3 l = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.Warp(l);

            float speed = Ref.FunnelParams.MoveSpeed.Expand;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * speed;
            _lerp = Mathf.Clamp01(_lerp);
        }

        // 移動完了。
        bool IsMoveCompleted()
        {
            return _lerp >= 1;
        }
    }
}
