using Enemy.Extensions;
using UnityEngine;

namespace Enemy.Control.BT
{
    /// <summary>
    /// スロットの位置に向けて移動する。
    /// </summary>
    public class MoveToPlayer : Node
    {
        private Choice _choice;
        private Transform _transform;
        private Transform _rotate;
        private BlackBoard _blackBoard;
        private EnemyParams _params;

        private float _speed;

        public MoveToPlayer(Choice choice, Transform transform, Transform rotate, BlackBoard blackBoard, 
            EnemyParams enemyParams)
        {
            _choice = choice;
            _transform = transform;
            _rotate = rotate;
            _blackBoard = blackBoard;
            _params = enemyParams;

            // 接近か否かで速さが変わる。
            // 現状、接近か戦闘しかないのでこれで十分。
            _speed = choice == Choice.Approach ? enemyParams.Advance.MoveSpeed : enemyParams.Battle.ChaseSpeed;
        }

        protected override void OnBreak()
        {
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override State Stay()
        {
            // 必要な参照が無い場合は、その場に留まる。
            if (_blackBoard.Slot == null)
            {
                _blackBoard.AddMovementOption(Choice.Chase, Vector3.zero, 0);
                return State.Running;
            }

            // キャラクターの向きに関係なく、エリアをホーミングでスロットに近づける。
            // エリア自体は毎フレームPerceptionでTransformの位置にリセットされる。
            Vector3 toSlot = VectorExtensions.Homing(
                _blackBoard.Area.Point, 
                _blackBoard.Slot.Point, 
                _blackBoard.AreaToSlotDirection,
                EnemyParams.Debug.HomingPower
                );

            // レベルの調整した速さ
            // -1から1の値を速さに乗算し、この値を速さに足すことで、0から基準値の2倍の範囲で加速減速を表現する。
            float orderedSpeed = _speed + _speed * _blackBoard.LevelAdjust.MoveSpeed;
            // エリアの中心位置からスロット方向へ1フレームぶん移動した位置へワープさせる。
            // エリアの半径が小さすぎない限り、移動させても飛び出すことは無い。
            Vector3 delta = toSlot * orderedSpeed * BlackBoard.DeltaTime;

            Vector3 warp;
            if (delta.sqrMagnitude >= _blackBoard.AreaToSlotSqrDistance)
            {
                warp = _blackBoard.Slot.Point;
            }
            else
            {
                warp = _blackBoard.Area.Point + delta;
            }
            // y座標はプレイヤーの位置に少しずつ移動する。
            warp.y = Mathf.Lerp(
                _transform.position.y, 
                _blackBoard.PlayerPosition.y, 
                EnemyParams.Debug.VerticalMoveSpeed * BlackBoard.DeltaTime
                );

            _blackBoard.AddWarpOption(_choice, warp);

            return State.Success;
        }
    }
}