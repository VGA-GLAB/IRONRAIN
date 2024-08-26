using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 画面に表示され、スロット位置まで接近するステート。
    /// </summary>
    public class ApproachState : PlayableState
    {
        private Vector3 _spawnPoint;
        private float _lerp;

        public ApproachState(RequiredRef requiredRef) : base(requiredRef) { }

        protected override void Always()
        {

        }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Approach;

            _spawnPoint = Ref.Body.Position;

            //Vector3 initVelo = 
            //Ref.BlackBoard.Velocity = 

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

            // 接近完了フラグ。
            Ref.BlackBoard.IsApproachCompleted = true;

            MoveToSlot();
        }

        protected override void Stay()
        {
            PlayDamageSE();

            if (ExitIfDeadOrTimeOver()) return;

            Vector3 before = Ref.Body.Position;
            MoveToSlot();
            LookAtPlayer();
            Vector3 after = Ref.Body.Position;
            MoveAnimation(after - before);

            if (IsMoveCompleted()) TryChangeState(StateKey.Battle);
        }

        // Lerpで移動。
        private void MoveToSlot()
        {
            Vector3 bp = _spawnPoint;
            Vector3 sp = Ref.BlackBoard.Slot.Point;
            Vector3 l = Vector3.Lerp(bp, sp, _lerp);
            Ref.Body.Warp(l);

            float speed = Ref.EnemyParams.MoveSpeed.Chase;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * speed;
            _lerp = Mathf.Clamp01(_lerp);
        }

        // プレイヤーを向かせる。
        private void LookAtPlayer()
        {
            Vector3 dir = Ref.BlackBoard.PlayerDirection;
            dir.y = 0;

            Ref.Body.LookForward(dir);
        }

        // 移動完了。
        private bool IsMoveCompleted()
        {
            return _lerp >= 1;
        }
    }
}