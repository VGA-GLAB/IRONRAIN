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
        private BossActionStep[] _steps;
        private BattleActionStep _currentStep;

        public LaneChangeState(RequiredRef requiredRef) : base(requiredRef)
        {
            _steps = new BossActionStep[2];
            _steps[1] = new LaneChangeEndStep(requiredRef, null);
            _steps[0] = new LaneChangeStep(requiredRef, _steps[1]);
        }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.LaneChange;

            _currentStep = _steps[0];
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            _currentStep = _currentStep.Update();

            if (_currentStep.ID == nameof(LaneChangeEndStep)) TryChangeState(StateKey.Idle);
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
            int pl = Ref.Field.CurrentRane.Value;
            int count = Ref.Field.LaneList.Count;
            int target = (pl + (count / 2)) % count;

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
            else
            {
                Vector3 p = Vector3.Lerp(_start, _end, _lerp);
                Ref.Body.Warp(p);

                // レーン間の移動速度。
                const float Speed = 3.0f;

                float dt = Ref.BlackBoard.PausableDeltaTime;
                _lerp += dt * Speed;
                _lerp = Mathf.Clamp01(_lerp);
            }

            return this;
        }

        // 円状に敷き詰められたレーンの2点間を移動する場合の移動回数を求める。
        // 1,2,3...17,0のように、末尾から先頭に移動する際に、変化量が変わるのを考慮して計算する。
        // aとbを入れ替えると時計回り/反時計回りの移動回数が求まる。
        private int GetRest(int a, int b)
        {
            int length = Ref.Field.LaneList.Count;
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
