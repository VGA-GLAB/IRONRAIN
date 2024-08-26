using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel
{
    public class ReturnState : State<StateKey>
    {
        // Lerpで動かす。
        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;

        public ReturnState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        protected RequiredRef Ref { get; private set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Return;

            AgentScript agent = Ref.AgentScript;
            if (agent != null) agent.EnemyDestory();

            _start = Ref.Transform.position;
            _lerp = 0;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            UpdateLerpEnd();
            MoveToOffsetedPoint();

            if (IsMoveCompleted()) TryChangeState(StateKey.Hide);
        }

        // ボスが動いているので、Enterのタイミングで戻る位置を固定できない。
        private void UpdateLerpEnd()
        {
            _end = Ref.Boss.transform.position;
        }

        // Lerpで移動。
        private void MoveToOffsetedPoint()
        {
            Vector3 l = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.Warp(l);

            float speed = Ref.FunnelParams.MoveSpeed.Return;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * speed;
            _lerp = Mathf.Clamp01(_lerp);
        }

        // 移動完了。
        private bool IsMoveCompleted()
        {
            return _lerp >= 1;
        }
    }
}
