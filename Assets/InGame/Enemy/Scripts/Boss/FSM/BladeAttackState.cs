using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss.FSM
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
        private BattleActionStep _chargeJetPack;
        private BattleActionStep _approachToPlayer;
        private BattleActionStep _gedanGiri;
        private BattleActionStep _chargeThrust;
        private BattleActionStep _passing;
        private BattleActionStep _bladeCooldown;
        private BattleActionStep _bladeAttackEnd;

        private BattleActionStep _currentStep;

        public BladeAttackState(RequiredRef requiredRef) : base(requiredRef)
        {
            PreAttackData preAttackData = new PreAttackData();

            _bladeAttackEnd = new BladeAttackEndStep();
            _bladeCooldown = new BladeCooldownStep(requiredRef, _bladeAttackEnd);
            _passing = new PassingStep(requiredRef, _bladeCooldown, preAttackData);
            _gedanGiri = new GedanGiriStep(requiredRef, _passing);
            _chargeThrust = new ChargeThrustStep(requiredRef, _passing);
            _approachToPlayer = new ApproachToPlayerStep(requiredRef, _gedanGiri, _chargeThrust, preAttackData);
            _chargeJetPack = new ChargeJetPackStep(requiredRef, _approachToPlayer);
        }

        protected override void Enter()
        {
            _blackBoard.CurrentState = StateKey.BladeAttack;
            _currentStep = _chargeJetPack;
        }

        protected override void Exit()
        {
            _chargeJetPack.Reset();
            _approachToPlayer.Reset();
            _gedanGiri.Reset();
            _chargeThrust.Reset();
            _passing.Reset();
            _bladeCooldown.Reset();
            _bladeAttackEnd.Reset();
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
        }
    }

    /// <summary>
    /// ジェットパックが光ってチャージ。
    /// </summary>
    public class ChargeJetPackStep : BattleActionStep
    {
        private BlackBoard _blackBoard;
        private Body _body;
        private BodyAnimation _animation;
        private BattleActionStep _approachToPlayer;

        private float _timer;

        public ChargeJetPackStep(RequiredRef requiredRef, BattleActionStep approachToPlayer)
        {
            _blackBoard = requiredRef.BlackBoard;
            _body = requiredRef.Body;
            _animation = requiredRef.BodyAnimation;
            _approachToPlayer = approachToPlayer;
        }

        public override string ID => nameof(ChargeJetPackStep);

        protected override void Enter()
        {
            _timer = 1.0f; // 現状、チャージ演出がないので適当
        }

        protected override BattleActionStep Stay()
        {
            // プレイヤーの方を向く。
            Vector3 f = _blackBoard.PlayerDirection;
            f.y = 0;
            _body.LookForward(f);

            // 時間が来たら遷移。
            _timer -= _blackBoard.PausableDeltaTime;
            if (_timer > 0) return this;
            else return _approachToPlayer;
        }
    }

    /// <summary>
    /// 自機に接近。
    /// </summary>
    public class ApproachToPlayerStep : BattleActionStep
    {
        private BossParams _params;
        private BlackBoard _blackBoard;
        private Body _body;
        private BodyAnimation _animation;
        private BattleActionStep _gedanGiri;
        private BattleActionStep _chargeThrust;
        private PreAttackData _preAttackData;
        private BossParams.Skill _selected;

        public ApproachToPlayerStep(RequiredRef requiredRef, 
            BattleActionStep gedanGidi, BattleActionStep chargeThrust, PreAttackData preAttackData)
        {
            _params = requiredRef.BossParams;
            _blackBoard = requiredRef.BlackBoard;
            _body = requiredRef.Body;
            _animation = requiredRef.BodyAnimation;
            _gedanGiri = gedanGidi;
            _chargeThrust = chargeThrust;
            _preAttackData = preAttackData;
        }

        public override string ID => nameof(ApproachToPlayerStep);

        protected override void Enter()
        {
            // 攻撃後、同じ位置関係に戻っている必要があるため、現在の位置関係を保存しておく。
            _preAttackData.Direction = _blackBoard.PlayerDirection;
            _preAttackData.SqrDistance = _blackBoard.PlayerSqrDistance;
            _preAttackData.PlayerPosition = _blackBoard.PlayerArea.Point;
            
            // 技ごとに接近距離を設定しているので、このタイミングでどの技を使うか決める。
            if (Random.value < 0.5f) _selected = _params.MeleeAttackConfig.GedanGiri;
            else _selected = _params.MeleeAttackConfig.ChargeThrust;
        }

        protected override BattleActionStep Stay()
        {
            float sqrDist = (_blackBoard.Area.Point - _preAttackData.PlayerPosition).sqrMagnitude;

            // プレイヤーとの距離を詰める。
            if (sqrDist > _selected.SprDistance)
            {
                Vector3 velo =
                    _preAttackData.Direction * 
                    _params.MeleeAttackConfig.ChargeSpeed * 
                    _blackBoard.PausableDeltaTime;
                _body.Move(velo);

                return this;
            }
            else
            {
                // Enterのタイミングで決定した技に遷移。
                if (_selected.ID == nameof(BossParams.GedanGiri)) return _gedanGiri;
                else return _chargeThrust;
            }
        }
    }

    /// <summary>
    /// 下段斬り。
    /// </summary>
    public class GedanGiriStep : BattleActionStep
    {
        private BodyAnimation _animation;
        private BattleActionStep _passing;

        public GedanGiriStep(RequiredRef requiredRef, BattleActionStep passing)
        {
            _animation = requiredRef.BodyAnimation;
            _passing = passing;
        }

        public override string ID => nameof(GedanGiriStep);

        protected override void Enter()
        {
            _animation.SetTrigger(BodyAnimationConst.Param.BladeAttack);
            _animation.ResetTrigger(BodyAnimationConst.Param.AttackSet);
        }

        protected override BattleActionStep Stay()
        {
            return _passing;
        }
    }

    /// <summary>
    /// 溜めからの突き。
    /// </summary>
    public class ChargeThrustStep : BattleActionStep
    {
        private BodyAnimation _animation;
        private BattleActionStep _passing;

        public ChargeThrustStep(RequiredRef requiredRef, BattleActionStep passing)
        {
            _animation = requiredRef.BodyAnimation;
            _passing = passing;
        }

        public override string ID => nameof(ChargeThrustStep);

        protected override void Enter()
        {
            // 現状、モーションが出来ていないので下斬りと同じ物を再生しておく。
            _animation.SetTrigger(BodyAnimationConst.Param.BladeAttack);
            _animation.ResetTrigger(BodyAnimationConst.Param.AttackSet);
        }

        protected override BattleActionStep Stay()
        {
            return _passing;
        }
    }

    /// <summary>
    /// 自機とすれ違う。
    /// </summary>
    public class PassingStep : BattleActionStep
    {
        private BossParams _params;
        private BlackBoard _blackBoard;
        private Body _body;
        private BodyAnimation _animation;
        private BattleActionStep _bladeCooldown;
        private PreAttackData _preAttackData;

        public PassingStep(RequiredRef requiredRef, BattleActionStep bladeCooldown, PreAttackData preAttackData)
        {
            _params = requiredRef.BossParams;
            _blackBoard = requiredRef.BlackBoard;
            _body = requiredRef.Body;
            _animation = requiredRef.BodyAnimation;
            _bladeCooldown = bladeCooldown;
            _preAttackData = preAttackData;
        }

        public override string ID => nameof(PassingStep);

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            float sqrDist = (_blackBoard.Area.Point - _preAttackData.PlayerPosition).sqrMagnitude;
            if (sqrDist < _preAttackData.SqrDistance)
            {
                Vector3 velo = 
                    _preAttackData.Direction * 
                    _params.MeleeAttackConfig.ChargeSpeed * 
                    _blackBoard.PausableDeltaTime;
                _body.Move(velo);

                return this;
            }
            else
            {
                return _bladeCooldown;
            }
        }
    }

    /// <summary>
    /// アニメーションの終了を待って攻撃終了させる。
    /// </summary>
    public class BladeCooldownStep : BattleActionStep
    {
        private BlackBoard _blackBoard;
        private BattleActionStep _bladeAttackEnd;

        private float _timer;

        public BladeCooldownStep(RequiredRef requiredRef, BattleActionStep bladeAttackEnd)
        {
            _blackBoard = requiredRef.BlackBoard;
            _bladeAttackEnd = bladeAttackEnd;
        }

        public override string ID => nameof(LauncherCooldownStep);

        protected override void Enter()
        {
            _timer = 4.5f; // アニメーションに合う用に手動で設定。
        }

        protected override BattleActionStep Stay()
        {
            _timer -= _blackBoard.PausableDeltaTime;
            if (_timer <= 0) return _bladeAttackEnd;
            else return this;
        }
    }

    /// <summary>
    /// 刀攻撃終了。
    /// </summary>
    public class BladeAttackEndStep : BattleActionStep
    {
        public override string ID => nameof(BladeAttackEndStep);

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            return this;
        }
    }
}
