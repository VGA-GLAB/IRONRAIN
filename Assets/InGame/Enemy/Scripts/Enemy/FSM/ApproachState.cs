using UnityEngine;

namespace Enemy.FSM
{
    /// <summary>
    /// 画面に表示され、スロット位置まで接近するステート。
    /// </summary>
    public class ApproachState : BattleState
    {
        private Effector _effector;
        private AgentScript _agentScript;

        public ApproachState(RequiredRef requiredRef) : base(requiredRef)
        {
            _effector = requiredRef.Effector;
            _agentScript = requiredRef.AgentScript;
        }

        protected override void Enter()
        {
            _blackBoard.CurrentState = StateKey.Approach;

            // 生成位置へワープする。
            if(_blackBoard.SpawnPoint != null)
            {
                _body.Warp((Vector3)_blackBoard.SpawnPoint);
            }

            // レーダーマップに表示させる。
            if (_agentScript != null) _agentScript.EnemyGenerate();

            // スラスター、トレイルの有効化。
            _effector.ThrusterEnable(true);
            _effector.TrailEnable(true);
        }

        protected override void Exit()
        {
            // 接近アニメーション終了をトリガー。
            _animation.SetTrigger(BodyAnimationConst.Param.ApproachEnd);
        }

        protected override void Stay()
        {
            // 継承元のBattleStateクラス、雑魚敵の共通したメソッド群。
            PlayDamageSE();
            if (BattleExit()) return;
            MoveToSlot(_params.MoveSpeed.Approach);

            // 接近アニメーション終了は次フレームでExitが呼ばれたタイミング。
            if (_blackBoard.IsApproachCompleted) TryChangeState(StateKey.Battle);
        }
    }
}