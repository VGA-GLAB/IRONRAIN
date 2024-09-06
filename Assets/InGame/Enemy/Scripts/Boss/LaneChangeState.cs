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
        private BossActionStep[] _laneChangeSteps;
        private BattleActionStep _currentLaneChangeStep;
        // 並行してプレイヤーの方向を向く回転を行う。
        private BossActionStep[] _lookSteps;
        private BattleActionStep _currentLookStep;

        public LaneChangeState(RequiredRef requiredRef) : base(requiredRef)
        {
            _laneChangeSteps = new BossActionStep[2];
            _laneChangeSteps[1] = new EndStep(requiredRef, null);
            _laneChangeSteps[0] = new LaneChangeStep(requiredRef, _laneChangeSteps[1]);

            _lookSteps = new BossActionStep[2];
            _lookSteps[1] = new EndStep(requiredRef, null);
            _lookSteps[0] = new LookAtPlayerStep(requiredRef, _lookSteps[1]);
        }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.LaneChange;

            _currentLaneChangeStep = _laneChangeSteps[0];
            _currentLookStep = _lookSteps[0];
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            _currentLaneChangeStep = _currentLaneChangeStep.Update();
            _currentLookStep = _currentLookStep.Update();

            bool isLaneChangeEnd = _currentLaneChangeStep.ID == nameof(EndStep);
            bool isLookEnd = _currentLookStep.ID == nameof(EndStep);
            if (isLaneChangeEnd && isLookEnd) TryChangeState(StateKey.Idle);
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

            // 既にプレイヤーの反対レーンにいる場合は移動しない。
            if (_rest > 0) NextLane();
            else
            {
                _start = Ref.Body.Position;
                _end = _start;
                _lerp = 0;
                _nextIndex = Ref.BlackBoard.CurrentLaneIndex;
            }
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
            const float Speed = 6.0f;

            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * Speed;
            _lerp = Mathf.Clamp01(_lerp);

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
    /// プレイヤーに向けて回転。
    /// </summary>
    public class LookAtPlayerStep : BossActionStep
    {
        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;
        private int _diff;

        public LookAtPlayerStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            _start = Ref.Body.Forward;
            // ボス戦のフィールドの中心からプレイヤーのレーンへのベクトル。
            // レーン移動終了時にはこの正面のレーンに移動する予定なので、この方向を向く。
            Vector3 pl = Ref.Field.GetPlayerLane();
            pl.y = 0;
            _end = pl;
            _lerp = 0;

            _diff = Ref.Field.GetMinMoveCount();
        }

        protected override BattleActionStep Stay()
        {
            Vector3 look = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.LookForward(look);

            // 振り向き速度。
            const float Speed = 100.0f;

            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * (Speed / _diff);

            if (_lerp > 1.0f) return Next[0];
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