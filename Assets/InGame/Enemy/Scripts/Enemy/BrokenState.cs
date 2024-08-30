using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 撃破されたステート。
    /// </summary>
    public class BrokenState : State<StateKey>
    {
        private EnemyActionStep[] _steps;
        private BattleActionStep _currentStep;

        public BrokenState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;

            _steps = new EnemyActionStep[2];
            _steps[1] = new BrokenEndStep(Ref, null);
            _steps[0] = new BrokenEffectStep(Ref, _steps[1]);

            _currentStep = _steps[0];
        }

        private RequiredRef Ref { get; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Broken;

            // レーダーマップから消す。
            AgentScript agent = Ref.AgentScript;
            if (agent != null) agent.EnemyDestory();

            // プレイヤーとの相対位置に移動させる。
            Vector3 rp = Ref.BlackBoard.PlayerRelativePosition;
            Vector3 pp = Ref.Player.position;
            Ref.Body.Warp(pp + rp);
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            _currentStep = _currentStep.Update();

            if (_currentStep.ID == nameof(BrokenEndStep))
            {
                TryChangeState(StateKey.Delete);
            }
        }
    }

    /// <summary>
    /// 破壊された際の演出を再生。
    /// </summary>
    public class BrokenEffectStep : EnemyActionStep
    {
        // Lerpでプレイヤーの後方まで徐々に移動させる。
        private float _start;
        private float _end;
        private float _diff;
        private float _lerp;
        // 重力に従って落下させる。
        private float _gravity;

        public BrokenEffectStep(RequiredRef requiredRef, params EnemyActionStep[] next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            // 再生するアニメーション名が敵の種類によって違う。
            EnemyType type = Ref.EnemyParams.Type;
            string stateName = "";
            if (type == EnemyType.Assault) stateName = Const.Assault.Damage;
            else if (type == EnemyType.Launcher) stateName = Const.Launcher.Damage;
            else if (type == EnemyType.Shield) stateName = Const.Shield.Damage;

            // 死亡アニメーションはその瞬間に強制的に遷移させるため、ステートを指定して再生。
            int layer = Const.Layer.BaseLayer;
            Ref.BodyAnimation.Play(stateName, layer);

            AudioWrapper.PlaySE("SE_Kill");

            Ref.Effector.PlayDestroyedEffect();
            Ref.Effector.ThrusterEnable(false);
            Ref.Effector.TrailEnable(false);
            Ref.Body.HitBoxEnable(false);

            // LerpでZ軸方向に移動させる距離。
            const float KnockBack = 10.0f;

            float z = Ref.Body.Position.z;
            float pz = Ref.Player.position.z;
            _start = z - pz;
            _end = _start + KnockBack;
            _diff = Mathf.Abs(_start - _end);
            _lerp = 0;
            _gravity = 0;

            Always();
        }

        protected override BattleActionStep Stay()
        {
            Always();

            // 0より大きい値を設定すると、後方への移動が完了後、落下し続ける。
            const float Delay = 1.0f;

            if (_lerp >= 1 + Delay) return Next[0];
            else return this;
        }

        private void Always()
        {
            // 生成位置からスロットの位置をLerpで動かす。
            float l = Mathf.Lerp(_start, _end, _lerp);
            float pz = Ref.Player.position.z;
            Vector3 p = Ref.Body.Position;
            p.z = pz + l;

            // 重力の強さ。
            const float Gravity = 0.98f * 30.0f;

            // 重力に従って落下させる。
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _gravity += dt * Gravity;
            p.y -= _gravity * dt;

            Ref.Body.Warp(p);

            // 後方への移動速度。
            const float Speed = 10.0f;

            // Lerpの補間値を更新。距離が変わっても一定の速度で移動させる。
            _lerp += Speed / _diff * dt;          
        }
    }

    /// <summary>
    /// 破壊演出終了、画面から消しても良い。
    /// </summary>
    public class BrokenEndStep : EnemyActionStep
    {
        public BrokenEndStep(RequiredRef requiredRef, params EnemyActionStep[] next) : base(requiredRef, next)
        {
        }
    }
}
