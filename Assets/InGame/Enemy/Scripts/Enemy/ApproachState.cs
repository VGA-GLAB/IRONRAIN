using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 準備完了後、スロット位置まで接近するステート。
    /// </summary>
    public class ApproachState : PlayableState
    {
        private Vector3 _start;
        private float _diff;
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

            // Lerpで等速運動。
            Vector3 p = Ref.Body.Position;
            Vector3 sp = Ref.BlackBoard.Slot.Point;
            _start = p - sp;
            _diff = _start.magnitude;
            _lerp = 0;

            // SEの再生。
            int thrusterSe = AudioWrapper.PlaySE(p, "SE_Thruster");
            int jetSe = AudioWrapper.PlaySE(p, "SE_Jet");
            Ref.BlackBoard.ThrusterSE = thrusterSe;
            Ref.BlackBoard.JetSE = jetSe;

            Always();
        }

        protected override void Exit()
        {
            Always();

            // 接近アニメーション終了をトリガー。
            Ref.BodyAnimation.SetTrigger(Const.Param.ApproachEnd);

            // 接近完了フラグ。
            Ref.BlackBoard.IsApproachCompleted = true;

            WriteBrokenPosition();
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
            Vector3 l = Vector3.Lerp(_start, Vector3.zero, _lerp);
            Vector3 p = Ref.BlackBoard.Slot.Point + l;
            Ref.Body.Warp(p);
            
            Vector3 after = Ref.Body.Position;

            // 移動の前後で位置を比較し、移動方向によってアニメーション。
            LeftRightMoveAnimation(after - before);

            // Lerpの補間値を更新。距離が変わっても一定の速度で移動させる。
            float speed = Ref.EnemyParams.MoveSpeed.Approach;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += speed / _diff * dt;
            _lerp = Mathf.Clamp01(_lerp);

            // プレイヤーを向かせる。
            Vector3 dir = Ref.BlackBoard.PlayerDirection;
            dir.y = 0;
            Ref.Body.LookForward(dir);

            // SEの位置更新
            UpdateSePosition();
        }
    }
}