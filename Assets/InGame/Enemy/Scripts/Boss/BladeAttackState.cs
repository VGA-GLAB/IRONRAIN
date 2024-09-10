using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Boss.BladeAttack;

namespace Enemy.Boss
{
    /// <summary>
    /// 左右どちらの向きで戻るかをステップ間で共有する。
    /// </summary>
    public class ReturnRoute
    {
        public bool IsRight { get; set; }
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
            ReturnRoute route = new ReturnRoute();

            _steps = new BossActionStep[6];
            _steps[5] = new EndStep(requiredRef, null);
            _steps[4] = new LookMyLaneForwardStep(requiredRef, _steps[5]);
            _steps[3] = new ReturnMyLaneStep(requiredRef, route, _steps[4]);
            _steps[2] = new LookLeftOrRightStep(requiredRef, route, _steps[3]);
            _steps[1] = new ChargeToPlayerStep(requiredRef, _steps[2]);
            _steps[0] = new ChargeJetPackStep(requiredRef, _steps[1]);
        }

        protected override void OnEnter()
        {
            Ref.BlackBoard.CurrentState = StateKey.BladeAttack;
            
            _currentStep = _steps[0];
        }

        protected override void OnExit()
        {
        }

        protected override void OnStay()
        {
            _currentStep = _currentStep.Update();

            if (_currentStep.ID == nameof(EndStep)) TryChangeState(StateKey.Idle);
        }

        public override void Dispose()
        {
            foreach (BattleActionStep s in _steps) s.Dispose();
        }
    }
}

namespace Enemy.Boss.BladeAttack
{
    /// <summary>
    /// ジェットパックが光ってチャージ。
    /// </summary>
    public class ChargeJetPackStep : BossActionStep
    {
        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;

        public ChargeJetPackStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            _start = Ref.Body.Forward;
            _end = Ref.BlackBoard.PlayerDirection;
            _lerp = 0;

            // 刀を展開。
            BladeEquipment blade = Ref.MeleeEquip as BladeEquipment;
            blade.Open();
        }

        protected override BattleActionStep Stay()
        {
            // プレイヤーを向く速度。
            const float Speed = 6.0f;

            Vector3 look = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.LookForward(look);

            if (_lerp >= 1.0f) return Next[0];

            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * Speed;
            _lerp = Mathf.Clamp01(_lerp);

            return this;
        }
    }

    /// <summary>
    /// プレイヤーに突進しつつ攻撃。
    /// </summary>
    public class ChargeToPlayerStep : BossActionStep
    {
        // プレイヤーとすれ違った後、ブレーキをかけ始めるタイミングを手動で指定。
        const float BrakeStartTiming = 1.5f;
        // AnimatinoClip全体の長さのうち、突進後の振り向きが終了する秒数を手動で指定。
        // 近接攻撃のアニメーション、最後に振り向いて棒立ちしているので、その部分をカットする意図。
        const float TurnEndTiming = 4.0f;

        private Vector3 _velocity;
        private float _timer;

        public ChargeToPlayerStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next)
        {
            // すれ違い後にターンするので、とりあえずこのステップで音の再生をトリガーしておく。
            requiredRef.AnimationEvent.OnTurn += OnTurn;
        }

        private void OnTurn()
        {
            Vector3 p = Ref.Body.Position;
            AudioWrapper.PlaySE(p, "SE_Turn_01");
        }

        protected override void Enter()
        {
            Ref.BodyAnimation.SetTrigger(Const.Param.BladeAttack);
            Ref.BodyAnimation.ResetTrigger(Const.Param.AttackSet);

            // 初速。
            const float InitialSpeed = 15.0f;

            _velocity = Ref.Body.Forward * InitialSpeed;
            _timer = 0;
        }

        protected override BattleActionStep Stay()
        {
            Vector3 acc;

            // プレイヤーとすれ違い、振り向きが終わって、ブレーキで完全に停止している状態。
            if (_timer >= TurnEndTiming)
            {
                Ref.BodyAnimation.SetTrigger(Const.Param.BladeAttackEnd);
                return Next[0];
            }
            else if (_timer >= BrakeStartTiming)
            {
                // ある程度速度が落ちたらピタっと止める。
                const float StopThreshold = 1.0f;

                if (_velocity.sqrMagnitude < StopThreshold)
                {
                    acc = Vector3.zero;
                    _velocity = Vector3.zero;
                }
                else
                {
                    // ブレーキの強さ。
                    const float Brake = 80.0f;

                    acc = -Ref.Body.Forward * Brake;
                }

            }
            else
            {
                // 加速度。
                const float Speed = 25.0f;

                acc = Ref.Body.Forward * Speed;
            }

            float dt = Ref.BlackBoard.PausableDeltaTime;
            _timer += dt;
            _velocity += acc * dt;
            Ref.Body.Move(_velocity * dt);

            return this;
        }

        public override void Dispose()
        {
            Ref.AnimationEvent.OnTurn -= OnTurn;
        }
    }

    /// <summary>
    /// 元いたレーンに戻るため、振り向いた状態から左右どちらかを向く。
    /// </summary>
    public class LookLeftOrRightStep : BossActionStep
    {
        private ReturnRoute _route;

        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;

        public LookLeftOrRightStep(RequiredRef requiredRef, ReturnRoute route, BossActionStep next)
            : base(requiredRef, next)
        {
            _route = route;
        }

        protected override void Enter()
        {
            _start = Ref.Body.Forward;

            // 左右どちらに移動するかはとりあえずランダム。
            if (Random.value < 0.5f)
            {
                _end = Ref.Body.Right;
                _route.IsRight = true;
            }
            else
            {
                _end = -Ref.Body.Right;
                _route.IsRight = false;
            }

            _lerp = 0;
        }

        protected override BattleActionStep Stay()
        {
            Vector3 look = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.LookForward(look);

            if (_lerp >= 1.0f) return Next[0];

            // 振り向き速度。
            const float Speed = 3.0f;

            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * Speed;
            _lerp = Mathf.Clamp01(_lerp);

            return this;
        }
    }

    /// <summary>
    /// 半円を描く軌道で攻撃前にいたレーンに戻る。
    /// </summary>
    public class ReturnMyLaneStep : BossActionStep
    {
        // 左右どちらの方向に戻るか読み取る。
        private ReturnRoute _route;

        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;
        private Vector3 _forward;
        private Vector3 _side;
        private Vector3 _center;
        private float _radius;

        public ReturnMyLaneStep(RequiredRef requiredRef, ReturnRoute route, BossActionStep next)
            : base(requiredRef, next)
        {
            _route = route;
        }

        protected override void Enter()
        {
            _start = Ref.Body.Position;
            _end = Ref.Field.GetCurrentLanePointWithOffset();
            _lerp = 0;

            // 現在地とレーンの位置を結ぶ直線を直径とする円。
            _center = (_start + _end) / 2;
            _radius = (_start - _end).magnitude / 2;

            Vector3 dir = _start - _end;
            _forward = dir.normalized;
            Vector3 route = _route.IsRight ? Vector3.up : Vector3.down;
            _side = Vector3.Cross(dir, route).normalized;
        }

        protected override BattleActionStep Stay()
        {
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt;
            _lerp = Mathf.Clamp01(_lerp);

            if (_lerp >= 1.0f) return Next[0];

            float theta = Mathf.Lerp(0, Mathf.PI, _lerp);
            float cos = Mathf.Cos(theta);
            float sin = Mathf.Sin(theta);
            Vector3 forward = _forward * cos * _radius;
            Vector3 side = _side * sin * _radius;
            Vector3 before = Ref.Body.Position;
            Ref.Body.Warp(_center + forward + side);
            Vector3 after = Ref.Body.Position;
            Ref.Body.LookForward(after - before);

            return this;
        }
    }

    /// <summary>
    /// 自身のレーンの正面を向く。
    /// </summary>
    public class LookMyLaneForwardStep : BossActionStep
    {
        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;

        public LookMyLaneForwardStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            _start = Ref.Body.Forward;
            _end = -Ref.Field.GetCurrentLane();
            _lerp = 0;

            // 刀をしまう。
            BladeEquipment blade = Ref.MeleeEquip as BladeEquipment;
            blade.Close();
        }

        protected override BattleActionStep Stay()
        {
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt;
            _lerp = Mathf.Clamp01(_lerp);

            if (_lerp >= 1.0f) return Next[0];

            Vector3 look = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.LookForward(look);

            return this;
        }
    }

    /// <summary>
    /// 近接攻撃終了。
    /// </summary>
    public class EndStep : BossActionStep
    {
        public EndStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            return this;
        }
    }
}