using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 撃破されたステート。
    /// </summary>
    public class BrokenState : ExitState
    {
        // 0より大きい値を設定すると、後方への移動が完了後、落下し続ける。
        const float Delay = 1.0f;

        // Lerpでプレイヤーの後方まで徐々に移動させる。
        private float _start;
        private float _end;
        private float _diff;
        private float _lerp;
        // 重力に従って落下させる。
        private float _gravity;

        public BrokenState(RequiredRef requiredRef) : base(requiredRef) { }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Broken;

            // レーダーマップから消す。
            AgentScript agent = Ref.AgentScript;
            if (agent != null) agent.EnemyIconDestory();

            // 再生するアニメーション名が敵の種類によって違う。
            EnemyType type = Ref.EnemyParams.Type;
            string stateName = "";
            if (type == EnemyType.Assault) stateName = Const.Assault.Damage;
            else if (type == EnemyType.Launcher) stateName = Const.Launcher.Damage;
            else if (type == EnemyType.Shield) stateName = Const.Shield.Damage;

            // 死亡アニメーションはその瞬間に強制的に遷移させるため、ステートを指定して再生。
            int layer = Const.Layer.BaseLayer;
            Ref.BodyAnimation.Play(stateName, layer);

            Vector3 p = Ref.Body.Position;
            AudioWrapper.PlaySE(p, "SE_Kill");

            int thruster = Ref.BlackBoard.ThrusterSE;
            int jet = Ref.BlackBoard.JetSE;
            AudioWrapper.StopSE(thruster);
            AudioWrapper.StopSE(jet);

            Ref.Effector.PlayDestroyedEffect();
            Ref.Effector.ThrusterEnable(false);
            Ref.Effector.TrailEnable(false);
            Ref.Body.HitBoxEnable(false);

            // LerpでZ軸方向に移動させる距離。
            const float KnockBack = 10.0f;

            _start = 0;
            _end = _start + KnockBack;
            _diff = Mathf.Abs(_start - _end);
            _lerp = 0;
            _gravity = 0;

            Always();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            Always();

            if (_lerp >= 1 + Delay) TryChangeState(StateKey.Delete);
        }

        private void Always()
        {
            // 生成位置からスロットの位置をLerpで動かす。
            float l = Mathf.Lerp(_start, _end, _lerp);
            float dt = Ref.BlackBoard.PausableDeltaTime;

            // ある程度アニメーションを再生させたら、重力に従って落下させる。
            const float DamageMotionEnd = 0.5f;

            if (_lerp > DamageMotionEnd)
            {
                // 重力の強さ。
                const float Gravity = 0.98f * 30.0f;

                _gravity += dt * Gravity;
                Vector3 p = Ref.Body.Position;
                p.y -= _gravity * dt;
                Ref.Body.Warp(p);
            }

            // 後方への移動速度。
            const float Speed = 5.0f;

            // Lerpの補間値を更新。距離が変わっても一定の速度で移動させる。
            _lerp += Speed / _diff * dt;

            // プレイヤーとの相対位置に移動させる。
            Vector3 sp = GetBasePosition();

            sp.x = Ref.Body.Position.x;
            sp.y = Ref.Body.Position.y;
            sp.z += l;
            Ref.Body.Warp(sp);
        }
    }
}