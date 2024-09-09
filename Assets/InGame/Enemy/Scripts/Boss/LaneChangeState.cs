using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Boss.LaneChange;

namespace Enemy.Boss
{
    /// <summary>
    /// プレイヤーのレーンに合わせてレーンを変更するステート。
    /// </summary>
    public class LaneChangeState : BattleState
    {
        // プレイヤーの正面の位置に来るよう、n回レーン変更を行う。
        private BossActionStep[] _steps;
        private BattleActionStep _currentStep;

        public LaneChangeState(RequiredRef requiredRef) : base(requiredRef)
        {
            _steps = new BossActionStep[3];
            _steps[2] = new EndStep(requiredRef, null);
            _steps[1] = new WaitAnimationStep(requiredRef, _steps[2]);
            _steps[0] = new LaneChangeStep(requiredRef, _steps[1]);
        }

        protected override void OnEnter()
        {
            Ref.BlackBoard.CurrentState = StateKey.LaneChange;

            _currentStep = _steps[0];

            TurnToPlayer(isReset: true);
        }

        protected override void OnExit()
        {
            TurnToPlayer();
        }

        protected override void OnStay()
        {
            TurnToPlayer();

            _currentStep = _currentStep.Update();

            if (_currentStep.ID == nameof(EndStep)) TryChangeState(StateKey.Idle);
        }

        #region ボスのレーン表示のテスト用。
        private bool _flag = false;
        private void DebugCube()
        {
            if (_flag) return;
            else _flag = true;

            for (int i = 0; i < Ref.Field.Length; i++)
            {
                Vector3 p = Ref.Field.GetLanePointWithOffset(i);
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                g.transform.position = p;
                g.name = "レーン" + i;
            }
        }
        #endregion
    }
}

namespace Enemy.Boss.LaneChange
{
    /// <summary>
    /// レーンを移動。
    /// </summary>
    public class LaneChangeStep : BossActionStep
    {
        // Lerpで移動。
        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;
        // このステップ内で複数回移動させる。
        private int _rest;
        private int _sign;
        // 移動開始のタイミングで代入、移動完了後にこの値を黒板に書き込む。
        private int _nextIndex;
        // 左右移動のアニメーションのブレンド値。
        private float _blend;

        public LaneChangeStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            // 現在のレーンから時計回りと反時計回りで移動する場合の移動回数が少ない方を選択。
            int clockwise = Ref.Field.GetClockwiseMoveCount();
            int counterclockwise = Ref.Field.GetCounterClockwiseMoveCount();
            if (clockwise <= counterclockwise)
            {
                // 時計回り
                _rest = clockwise;
                _sign = -1;
            }
            else
            {
                // 反時計回り
                _rest = counterclockwise;
                _sign = 1;
            }

            if (_rest > 0) NextLane();
            else
            {
                // 既にプレイヤーの反対レーンにいる場合は移動しない。
                _start = Ref.Body.Position;
                _end = _start;
                _lerp = 0;
                _sign = 0;
                _nextIndex = Ref.BlackBoard.CurrentLaneIndex;
            }

            _blend = 0;
        }

        protected override BattleActionStep Stay()
        {
            if (_lerp >= 1)
            {
                Ref.BlackBoard.CurrentLaneIndex = _nextIndex;

                if (_rest == 0) return Next[0];
                else NextLane();
            }

            Vector3 p = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.Warp(p);

            // レーン間の移動速度。
            const float MoveSpeed = 6.0f;

            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * MoveSpeed;
            _lerp = Mathf.Clamp01(_lerp);

            // アイドルから左右移動のアニメーションに切り替わる速さ。
            const float BlendSpeed = 3.0f;

            // _signには移動しない場合は0、左右に移動する場合は-1もしくは1が代入されている。
            // ブレンドツリーのパラメータをその値に徐々に変化させる。
            _blend = Mathf.Clamp(_blend, -1, 1);
            _blend = Mathf.MoveTowards(_blend, -_sign, dt * BlendSpeed);
            string param = Const.Param.SpeedX;
            Ref.BodyAnimation.SetFloat(param, _blend);

            return this;
        }

        // 移動先のレーンを更新。
        private void NextLane()
        {
            _rest--;

            if (_sign == -1) _nextIndex = Ref.Field.GetLeftLaneIndex();
            else if (_sign == 1) _nextIndex = Ref.Field.GetRightLaneIndex();

            _start = Ref.Body.Position;
            _end = Ref.Field.GetLanePointWithOffset(_nextIndex);
            _lerp = 0;
        }
    }

    /// <summary>
    /// 左右移動のアニメーションが終わるのを待つ。
    /// </summary>
    public class WaitAnimationStep : BossActionStep
    {
        private float _blend;

        public WaitAnimationStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            string param = Const.Param.SpeedX;
            _blend = Ref.BodyAnimation.GetFloat(param);
        }

        protected override BattleActionStep Stay()
        {
            // アイドルから左右移動のアニメーションに切り替わる速さ。
            const float BlendSpeed = 1.0f;

            float dt = Ref.BlackBoard.PausableDeltaTime;
            _blend = Mathf.MoveTowards(_blend, 0, dt * BlendSpeed);
            string param = Const.Param.SpeedX;
            Ref.BodyAnimation.SetFloat(param, _blend);

            if (Mathf.Approximately(_blend, 0)) return Next[0];
            else return this;
        }
    }

    /// <summary>
    /// レーン移動終了。
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