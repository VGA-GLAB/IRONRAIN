using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 画面外に逃げ出すステート
    /// </summary>
    public class EscapeState : State<StateKey>
    {
        public EscapeState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Escape;

            // レーダーマップから消す。
            AgentScript agent = Ref.AgentScript;
            if (agent != null) agent.EnemyDestory();
        }

        protected override void Exit()
        {
            // エフェクトを消す。
            Ref.Effector.ThrusterEnable(false);
            Ref.Effector.TrailEnable(false);
        }

        protected override void Stay()
        {
            // 上方向に移動。
            float spd = Ref.EnemyParams.MoveSpeed.Exit;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            Vector3 up = Vector3.up * spd * dt;
            Ref.Body.Move(up);

            // 画面外に出た場合は削除状態に遷移。
            float dist = Ref.BlackBoard.PlayerDistance;
            float offScreen = Ref.EnemyParams.Other.OffScreenDistance;
            if (dist > offScreen)
            {
                TryChangeState(StateKey.Delete);
            }
        }
    }
}
