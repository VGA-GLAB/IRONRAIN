using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss
{
    /// <summary>
    /// 攻撃前と後で同じ位置関係に戻す必要があるため、ステップ間で攻撃前のデータを共有する必要がある。
    /// </summary>
    public class PreAttackData
    {
        public Vector3 PlayerPosition { get; set; }
        public Vector3 Direction { get; set; }
        public float SqrDistance { get; set; }
    }

    /// <summary>
    /// 戦闘中、刀で攻撃するステート。
    /// </summary>
    public class BladeAttackState : BattleState
    {
        // チャージ->接近->下段斬りor溜め突き->自機とすれ違う
        private BossActionStep[] _steps;
        private BattleActionStep _currentStep;

        public BladeAttackState(RequiredRef requiredRef) : base(requiredRef)
        {
            PreAttackData preAttackData = new PreAttackData();

            _steps = new BossActionStep[7];
            _steps[6] = new BladeAttackEndStep(requiredRef, null);
            _steps[5] = new BladeCooldownStep(requiredRef, _steps[6]);
            _steps[4] = new PassingStep(requiredRef, preAttackData, _steps[5]);
            _steps[3] = new GedanGiriStep(requiredRef, _steps[4]);
            _steps[2] = new ChargeThrustStep(requiredRef, _steps[4]);
            _steps[1] = new ApproachToPlayerStep(requiredRef, preAttackData, _steps[2], _steps[3]);
            _steps[0] = new ChargeJetPackStep(requiredRef, _steps[1]);
        }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.BladeAttack;
            
            _currentStep = _steps[0];
        }

        protected override void Exit()
        {
            foreach (BattleActionStep s in _steps) s.Reset();
        }

        protected override void Stay()
        {
            PlayDamageSE();
            FunnelLaserSight();

            _currentStep = _currentStep.Update();

            // 終了ステップまで到達したらアイドル状態に戻る。
            if (_currentStep.ID == nameof(BladeAttackEndStep)) TryChangeState(StateKey.Idle);
        }

        public override void Dispose()
        {
            foreach (BattleActionStep s in _steps) s.Dispose();
        }
    }

    /// <summary>
    /// ジェットパックが光ってチャージ。
    /// </summary>
    public class ChargeJetPackStep : BossActionStep
    {
        private float _timer;

        public ChargeJetPackStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            _timer = 1.0f; // 現状、チャージ演出がないので適当
        }

        protected override BattleActionStep Stay()
        {
            // プレイヤーの方を向く。
            Vector3 f = Ref.BlackBoard.PlayerDirection;
            f.y = 0;
            Ref.Body.LookForward(f);

            // 時間が来たら遷移。
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _timer -= dt;
            if (_timer > 0) return this;
            else return Next[0];
        }
    }

    /// <summary>
    /// 自機に接近。
    /// </summary>
    public class ApproachToPlayerStep : BossActionStep
    {
        // 書き込んで後続のステップに渡す。
        private PreAttackData _preAttackData;
        // 遷移の判定用の値は技ごとに違う。
        private Skill _selected;

        public ApproachToPlayerStep(RequiredRef requiredRef, PreAttackData preAttackData,
            BossActionStep gedanGidi, BossActionStep chargeThrust) : base(requiredRef, gedanGidi, chargeThrust) 
        {
            _preAttackData = preAttackData;
        }

        protected override void Enter()
        {
            // 攻撃後、同じ位置関係に戻っている必要があるため、現在の位置関係を保存しておく。
            BlackBoard bb = Ref.BlackBoard;
            _preAttackData.Direction = bb.PlayerDirection;
            _preAttackData.SqrDistance = bb.PlayerSqrDistance;
            _preAttackData.PlayerPosition = bb.PlayerArea.Point;

            // 技ごとに接近距離を設定しているので、このタイミングでどの技を使うか決める。
            MeleeAttackSettings melee = Ref.BossParams.MeleeAttackConfig;
            if (Random.value < 0.5f) _selected = melee.GedanGiri;
            else _selected = melee.ChargeThrust;
        }

        protected override BattleActionStep Stay()
        {
            // Enterのタイミングのプレイヤーの位置、技の間合いまで近づく。
            Vector3 area = Ref.BlackBoard.Area.Point;
            Vector3 player = _preAttackData.PlayerPosition;
            float sqrDist = (area - player).sqrMagnitude;
            if (sqrDist > _selected.SprDistance)
            {
                Vector3 dir = _preAttackData.Direction;
                float spd = Ref.BossParams.MeleeAttackConfig.ChargeSpeed;
                float dt = Ref.BlackBoard.PausableDeltaTime;
                Vector3 velo = dir * spd * dt;
                Ref.Body.Move(velo);

                return this;
            }
            else
            {
                // Enterのタイミングで決定した技に遷移。
                bool isGedanGiri = _selected.ID == nameof(GedanGiri);
                if (isGedanGiri) return Next[0];
                else return Next[1];
            }
        }
    }

    /// <summary>
    /// 下段斬り。
    /// </summary>
    public class GedanGiriStep : BossActionStep
    {
        public GedanGiriStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.BladeAttack);
            Ref.BodyAnimation.ResetTrigger(BodyAnimationConst.Param.AttackSet);
        }

        protected override BattleActionStep Stay()
        {
            return Next[0];
        }
    }

    /// <summary>
    /// 溜めからの突き。
    /// </summary>
    public class ChargeThrustStep : BossActionStep
    {
        public ChargeThrustStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            // 現状、モーションが出来ていないので下斬りと同じ物を再生しておく。
            Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.BladeAttack);
            Ref.BodyAnimation.ResetTrigger(BodyAnimationConst.Param.AttackSet);
        }

        protected override BattleActionStep Stay()
        {
            return Next[0];
        }
    }

    /// <summary>
    /// 自機とすれ違う。
    /// </summary>
    public class PassingStep : BossActionStep
    {
        private PreAttackData _preAttackData;

        public PassingStep(RequiredRef requiredRef, PreAttackData preAttackData, BossActionStep next) : base(requiredRef, next)
        {
            _preAttackData = preAttackData;

            // すれ違い後にターンするので、とりあえずこのステップで音の再生をトリガーしておく。
            requiredRef.AnimationEvent.OnTurn += OnTurn;
        }

        private void OnTurn()
        {
            AudioWrapper.PlaySE("SE_Turn_01");
        }

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            // プレイヤーの座標まで直線移動、そのままの向きで一定距離離れる。
            Vector3 area = Ref.BlackBoard.Area.Point;
            Vector3 player = _preAttackData.PlayerPosition;
            float sqrDist = (area - player).sqrMagnitude;
            if (sqrDist < _preAttackData.SqrDistance)
            {
                Vector3 dir = _preAttackData.Direction;
                float spd = Ref.BossParams.MeleeAttackConfig.ChargeSpeed;
                float dt = Ref.BlackBoard.PausableDeltaTime;
                Vector3 velo = dir * spd * dt;
                Ref.Body.Move(velo);

                return this;
            }
            else
            {
                return Next[0];
            }
        }

        public override void Dispose()
        {
            Ref.AnimationEvent.OnTurn -= OnTurn;
        }
    }

    /// <summary>
    /// アニメーションの終了を待って攻撃終了させる。
    /// </summary>
    public class BladeCooldownStep : BossActionStep
    {
        private float _timer;

        public BladeCooldownStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            _timer = 4.5f; // アニメーションに合う用に手動で設定。
        }

        protected override BattleActionStep Stay()
        {
            LookAtPlayer();

            float dt = Ref.BlackBoard.PausableDeltaTime;
            _timer -= dt;
            if (_timer <= 0) return Next[0];
            else return this;
        }

        private void LookAtPlayer()
        {
            BlackBoard bb = Ref.BlackBoard;
            float dt = bb.PausableDeltaTime;

            bb.PlayerLookElapsed += dt;

            // ボスの前方向に射撃するファンネルを機能させるために一定間隔。
            float duration = Ref.BossParams.LookDuration;
            if (bb.PlayerLookElapsed > duration)
            {
                bb.PlayerLookElapsed = 0;

                Vector3 pd = bb.PlayerDirection;
                pd.y = 0;
                bb.LookDirection = pd;
            }

            const float LookSpeed = 5.0f; // 適当。
            Vector3 a = Ref.Body.Forward;
            Vector3 b = bb.LookDirection;
            Vector3 look = Vector3.Lerp(a, b, dt * LookSpeed);

            Ref.Body.LookForward(look);
        }
    }

    /// <summary>
    /// 刀攻撃終了。
    /// </summary>
    public class BladeAttackEndStep : BossActionStep
    {
        public BladeAttackEndStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            return this;
        }
    }
}
