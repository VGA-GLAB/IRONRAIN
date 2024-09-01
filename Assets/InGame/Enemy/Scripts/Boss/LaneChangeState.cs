using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            _laneChangeSteps[1] = new LaneChangeEndStep(requiredRef, null);
            _laneChangeSteps[0] = new LaneChangeStep(requiredRef, _laneChangeSteps[1]);

            _lookSteps = new BossActionStep[2];
            _lookSteps[1] = new LaneChangeEndStep(requiredRef, null);
            _lookSteps[0] = new LaneChangeLookAtPlayerStep(requiredRef, _lookSteps[1]);
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

            bool isLaneChangeEnd = _currentLaneChangeStep.ID == nameof(LaneChangeEndStep);
            bool isLookEnd = _currentLookStep.ID == nameof(LaneChangeEndStep);
            if (isLaneChangeEnd && isLookEnd) TryChangeState(StateKey.Idle);
        }

        #region ボスのレーン表示のテスト用。
        private bool _flag = false;
        private void DebugCube()
        {
            if (_flag) return;
            else _flag = true;

            for (int i = 0; i < Ref.Field.LaneList.Count; i++)
            {
                Vector3 p = Ref.PointP.position + Ref.Field.LaneList[i];
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                g.transform.position = p;
                g.name = "レーン" + i;
            }
        }
        #endregion
    }

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
            // プレイヤーの反対側のレーン。
            int target = GetOtherSideLane();

            // 現在のレーンから時計回りと反時計回りで移動する場合の移動回数が少ない方を選択。
            int current = Ref.BlackBoard.CurrentLaneIndex;
            int clockwiseLength = GetRest(target, current);
            int counterclockwiseLength = GetRest(current, target);
            if (clockwiseLength <= counterclockwiseLength)
            {
                // 時計回り
                _rest = clockwiseLength;
                _sign = 1;
            }
            else
            {
                // 反時計回り
                _rest = counterclockwiseLength;
                _sign = -1;
            }

            // 既にプレイヤーの反対レーンにいる場合は移動しない。
            if (_rest > 0) NextLane();
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

        // プレイヤーの反対側のレーンの番号を取得。
        private int GetOtherSideLane()
        {
            int pi = Ref.Field.CurrentRane.Value;
            int length = Ref.Field.LaneList.Count;
            return GetOtherSideLane(pi, length);
        }

        public static int GetOtherSideLane(int playerLaneIndex, int length)
        {
            return (playerLaneIndex + (length / 2)) % length;
        }

        // 円状に敷き詰められたレーンの2点間を移動する場合の移動回数を求める。
        // 1,2,3...17,0のように、末尾から先頭に移動する際に、変化量が変わるのを考慮して計算する。
        // aとbを入れ替えると時計回り/反時計回りの移動回数が求まる。
        private int GetRest(int a, int b)
        {
            int length = Ref.Field.LaneList.Count;
            return GetRest(a, b, length);
        }

        public static int GetRest(int a, int b, int length)
        {
            return (a - b + length) % length;
        }

        // 移動先のレーンを更新。
        private void NextLane()
        {
            _rest--;

            // 2点を指定して移動回数を求める計算と同じ。
            // なぜか現在の地点から移動方向を指定しても隣のレーンが求まる。
            int li = Ref.BlackBoard.CurrentLaneIndex;
            _nextIndex = GetRest(li, -_sign);

            _start = Ref.Body.Position;
            _end = Ref.PointP.position + Ref.Field.LaneList[_nextIndex];
            _lerp = 0;
        }
    }

    /// <summary>
    /// プレイヤーに向けて回転。
    /// </summary>
    public class LaneChangeLookAtPlayerStep : BossActionStep
    {
        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;
        private int _diff;

        public LaneChangeLookAtPlayerStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            _start = Ref.Body.Forward;
            // ボス戦のフィールドの中心からプレイヤーのレーンへのベクトル。
            // レーン移動終了時にはこの正面のレーンに移動する予定なので、この方向を向く。
            int pi = Ref.Field.CurrentRane.Value;
            Vector3 pl = Ref.Field.LaneList[pi];
            pl.y = 0;
            _end = pl;
            _lerp = 0;

            // プレイヤーの反対側のレーンに移動するため、向く方向も合わせる。
            int length = Ref.Field.LaneList.Count;
            int target = LaneChangeStep.GetOtherSideLane(pi, length);
            // 現在のレーンから時計回りと反時計回りで移動する場合の移動回数が少ない方を選択。
            int current = Ref.BlackBoard.CurrentLaneIndex;
            int clockwiseLength = LaneChangeStep.GetRest(target, current, length);
            int counterclockwiseLength = LaneChangeStep.GetRest(current, target, length);
            _diff = Mathf.Min(clockwiseLength, counterclockwiseLength);
        }

        protected override BattleActionStep Stay()
        {
            Vector3 look = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.LookForward(look);

            // 振り向き速度。
            const float Speed = 1.0f;

            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * (Speed / _diff);

            if (_lerp > 1.0f) return Next[0];
            else return this;
        }
    }

    /// <summary>
    /// レーン移動終了。
    /// </summary>
    public class LaneChangeEndStep : BossActionStep
    {
        public LaneChangeEndStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            return this;
        }
    }
}
