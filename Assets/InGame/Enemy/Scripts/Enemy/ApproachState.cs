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

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Approach;

            // レーダーマップに表示させる。
            AgentScript agent = Ref.AgentScript;
            if (agent != null) agent.EnemyGenerate();

            // スラスター、トレイルの有効化。
            Ref.Effector.ThrusterEnable(true);
            Ref.Effector.TrailEnable(true);

            // 隠れた状態から表示された瞬間の位置をLerpのaにする。
            _spawnPoint = Ref.Body.Position;
            _lerp = 0;

            Always();
        }

        protected override void Exit()
        {
            Always();

            // 接近アニメーション終了をトリガー。
            Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.ApproachEnd);

            // 接近完了フラグ。
            Ref.BlackBoard.IsApproachCompleted = true;
        }

        protected override void Stay()
        {
            if (ExitIfDeadOrTimeOver()) return;

            Always();

            // 補間値が1以上になった場合は移動完了とみなす。
            bool isCompleted = _lerp >= 1.0f;
            if (isCompleted) TryChangeState(StateKey.Battle);
        }

        // ダメージ音、移動、向く、移動アニメーション。
        private void Always()
        {
            PlayDamageSE();

            Vector3 before = Ref.Body.Position;

            // 生成位置からスロットの位置をLerpで動かす。
            Vector3 slotPoint = Ref.BlackBoard.Slot.Point;
            Vector3 l = Vector3.Lerp(_spawnPoint, slotPoint, _lerp);
            Ref.Body.Warp(l);

            Vector3 after = Ref.Body.Position;

            // 移動の前後で位置を比較し、移動方向によってアニメーション。
            MoveAnimation(after - before);

            // Lerpの補間値を更新。
            float speed = Ref.EnemyParams.MoveSpeed.Chase;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * speed;
            _lerp = Mathf.Clamp01(_lerp);

            // プレイヤーを向かせる。
            Vector3 dir = Ref.BlackBoard.PlayerDirection;
            dir.y = 0;
            Ref.Body.LookForward(dir);
        }
    }
}