using UnityEngine;

namespace Enemy.FSM
{
    /// <summary>
    /// 画面に表示され、スロット位置まで接近するステート。
    /// </summary>
    public class ApproachState : BattleState
    {
        public ApproachState(RequiredRef requiredRef) : base(requiredRef) { }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Approach;

            // 生成位置へワープする。
            Vector3? spawnPoint = Ref.BlackBoard.SpawnPoint;
            if(spawnPoint != null)
            {
                Ref.Body.Warp((Vector3)spawnPoint);
            }

            // レーダーマップに表示させる。
            AgentScript agent = Ref.AgentScript;
            if (agent != null) agent.EnemyGenerate();

            // スラスター、トレイルの有効化。
            Ref.Effector.ThrusterEnable(true);
            Ref.Effector.TrailEnable(true);
        }

        protected override void Exit()
        {
            // 接近アニメーション終了をトリガー。
            Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.ApproachEnd);
        }

        protected override void Stay()
        {
            // 継承元のBattleStateクラス、雑魚敵の共通したメソッド群。
            PlayDamageSE();

            if (BattleExit()) return;

            float spd = Ref.EnemyParams.MoveSpeed.Approach;
            MoveToSlot(spd);

            // 接近アニメーション終了は次フレームでExitが呼ばれたタイミング。
            bool isCompleted = Ref.BlackBoard.IsApproachCompleted;
            if (isCompleted) TryChangeState(StateKey.Battle);
        }
    }
}