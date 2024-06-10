using Enemy.Extensions;
using UnityEngine;

namespace Enemy.Control.BT
{
    /// <summary>
    /// スロットの位置に向けて移動する。
    /// </summary>
    public class MoveToPlayer : Node
    {
        private ActionPlan.Warp _plan;
        private Transform _transform;
        private EnemyParams _params;
        private BlackBoard _blackBoard;

        public MoveToPlayer(Choice choice, Transform transform, EnemyParams enemyParams, BlackBoard blackBoard)
        {
            _plan = new ActionPlan.Warp(choice);
            _transform = transform;
            _params = enemyParams;
            _blackBoard = blackBoard;
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
            if (_blackBoard.Slot == null) return State.Running;

            // キャラクターの向きに関係なく、エリアをホーミングでスロットに近づける。
            // エリア自体は毎フレームPerceptionでTransformの位置にリセットされる。
            Vector3 toSlot = VectorExtensions.Homing(
                _blackBoard.Area.Point, 
                _blackBoard.Slot.Point, 
                _blackBoard.AreaToSlotDirection,
                EnemyParams.Debug.HomingPower
                );

            // 接近か否かで速さが変わる。
            // 現状、接近か戦闘しかないのでこれで十分。
            float speed = _plan.Choice == Choice.Approach ? _params.Advance.MoveSpeed : _params.Battle.ChaseSpeed;
            // エリアの中心位置からスロット方向へ1フレームぶん移動した位置へワープさせる。
            // エリアの半径が小さすぎない限り、移動させても飛び出すことは無い。
            Vector3 delta = toSlot * speed * _blackBoard.PausableDeltaTime;

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
                EnemyParams.Debug.VerticalMoveSpeed * _blackBoard.PausableDeltaTime
            );

            _plan.Position = warp;
            _blackBoard.WarpPlans.Enqueue(_plan);

            return State.Success;
        }
    }
}