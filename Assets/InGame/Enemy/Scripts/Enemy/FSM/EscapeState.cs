using UnityEngine;

namespace Enemy.FSM
{
    /// <summary>
    /// 画面外に逃げ出すステート
    /// </summary>
    public class EscapeState : State
    {
        private EnemyParams _params;
        private BlackBoard _blackBoard;
        private Body _body;
        private Effector _effector;
        private AgentScript _agentScript;

        public EscapeState(StateRequiredRef requiredRef) : base(requiredRef.States)
        {
            _params = requiredRef.EnemyParams;
            _blackBoard = requiredRef.BlackBoard;
            _body = requiredRef.Body;
            _effector = requiredRef.Effector;
            _agentScript = requiredRef.AgentScript;
        }

        protected override void Enter()
        {
            _blackBoard.CurrentState = StateKey.Escape;

            // レーダーマップから消す。
            if (_agentScript != null) _agentScript.EnemyDestory();
        }

        protected override void Exit()
        {
            // エフェクトを消す。
            _effector.ThrusterEnable(false);
            _effector.TrailEnable(false);
        }

        protected override void Stay()
        {
            // 上方向に移動。
            Vector3 up = Vector3.up * _params.MoveSpeed.Exit * _blackBoard.PausableDeltaTime;
            _body.Move(up);

            // 画面外に出た場合は削除状態に遷移。
            if (_blackBoard.PlayerDistance > _params.Other.OffScreenDistance)
            {
                TryChangeState(StateKey.Delete);
            }
        }
    }
}
