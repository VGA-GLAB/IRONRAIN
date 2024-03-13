using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 行動をオブジェクトに反映してキャラクターを動かす。
    /// </summary>
    public class Action : ILifeCycleHandler
    {
        private BlackBoard _blackBoard;
        private BodyMove _move;
        private OffsetMove _offsetMove;
        private BodyRotate _rotate;
        private BodyAnimation _animation;

        public Action(Transform transform, Transform offset, Transform rotate, Animator animator, BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
            _move = new BodyMove(transform);
            _offsetMove = new OffsetMove(offset);
            _rotate = new BodyRotate(rotate);
            _animation = new BodyAnimation(animator);
        }

        public void UpdateEvent()
        {
            // deltaTimeぶんの移動を上書きする恐れがあるので、座標を直接書き換える処理を先にしておく。
            while (_blackBoard.WarpOptions.Count > 0)
            {
                DeltaWarp d = _blackBoard.WarpOptions.Dequeue();
                _move.Warp(d.Position);
            }

            // deltaTimeぶんの移動
            while (_blackBoard.MovementOptions.Count > 0)
            {
                DeltaMovement d = _blackBoard.MovementOptions.Dequeue();
                _move.Walk(d.Direction * d.Speed);
            }

            // ここでは非同期処理を解禁
            // 移動
            // 動的オフセット
            // 回転
            // アニメーション
        }
    }
}
