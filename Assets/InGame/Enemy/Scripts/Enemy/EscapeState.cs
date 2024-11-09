using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 画面外に逃げ出すステート
    /// </summary>
    public class EscapeState : ExitState
    {
        public EscapeState(RequiredRef requiredRef) : base(requiredRef) { }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Escape;

            // レーダーマップから消す。
            AgentScript agent = Ref.AgentScript;
            if (agent != null) agent.EnemyIconDestory();
            Always();
        }

        protected override void Exit()
        {
            // エフェクトを消す。
            Ref.Effector.ThrusterEnable(false);
            Ref.Effector.TrailEnable(false);

            int thruster = Ref.BlackBoard.ThrusterSE;
            int jet = Ref.BlackBoard.JetSE;
            AudioWrapper.StopSE(thruster);
            AudioWrapper.StopSE(jet);
        }

        protected override void Stay()
        {
            Always();

            // プレイヤーが見えない距離を適当に設定。
            const float OffScreenDist = 100.0f;

            // 画面外に出た場合は削除状態に遷移。
            float dist = Ref.BlackBoard.PlayerDistance;
            if (dist > OffScreenDist) TryChangeState(StateKey.Delete);
        }

        private void Always()
        {
            Vector3 bp = GetBasePosition();
            bp.y = Ref.Body.Position.y;
            Ref.Body.Warp(bp);

            // 上方向に移動。
            float spd = Ref.EnemyParams.MoveSpeed.Exit;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            Vector3 up = Vector3.up * spd * dt;
            Ref.Body.Move(up);

            // PlayableStateクラスのものと全く同じ処理、重複。
            Vector3 p = Ref.Body.Position;
            int thruster = Ref.BlackBoard.ThrusterSE;
            int jet = Ref.BlackBoard.JetSE;
            AudioWrapper.UpdateSePosition(p, thruster);
            AudioWrapper.UpdateSePosition(p, jet);
        }
    }
}