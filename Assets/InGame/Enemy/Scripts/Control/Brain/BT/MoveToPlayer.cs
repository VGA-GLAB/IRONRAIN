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

        public MoveToPlayer(Choice choice, Transform transform, Transform rotate, BlackBoard blackBoard, 
            EnemyParams enemyParams)
        {
            _choice = choice;
            _transform = transform;
            _rotate = rotate;
            _blackBoard = blackBoard;
            _params = enemyParams;
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

            // エリアの中心位置からスロット方向へ1フレームぶん移動した位置へワープさせる。
            // エリアの半径が小さすぎない限り、移動させても飛び出すことは無い。
            Vector3 delta = toSlot * _params.Move.ChaseSpeed * Time.deltaTime;
            if (delta.sqrMagnitude >= _blackBoard.AreaToSlotSqrDistance)
            {
                _blackBoard.AddWarpOption(_choice, _blackBoard.Slot.Point);
            }
            else
            {
                _blackBoard.AddWarpOption(_choice, _blackBoard.Area.Point + delta);
            }

            return State.Success;
        }
    }
}