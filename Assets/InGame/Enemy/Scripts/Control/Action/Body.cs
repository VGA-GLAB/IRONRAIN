using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 計算した値を実際にオブジェクトに反映する。
    /// </summary>
    public class Body
    {
        private Transform _transform;
        private BlackBoard _blackBoard;
        private Transform _offset;
        private Transform _rotate;
        private Renderer[] _renderers;

        public Body(Transform transform, BlackBoard blackBoard, Transform offset, Transform rotate, Renderer[] renderers)
        {
            _transform = transform;
            _blackBoard = blackBoard;
            _offset = offset;
            _rotate = rotate;
            _renderers = renderers;
        }

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 TransformPosition => _transform.position;

        /// <summary>
        /// オブジェクト自体を無効化して非表示にする。
        /// 撃破された際はこのメソッドで画面から消す。
        /// </summary>
        public void Disable()
        {
            _transform.gameObject.SetActive(false);
        }

        /// <summary>
        /// Rendererのみを表示/非表示にする。
        /// 生成後、撃破されていないが画面から消したい場合に有効。
        /// </summary>
        public void RendererEnable(bool value)
        {
            foreach (Renderer r in _renderers)
            {
                r.enabled = value;
            }
        }

        /// <summary>
        /// オブジェクトの座標を変更する。
        /// </summary>
        public void Warp(in Vector3 position)
        {
            _transform.position = position;
        }

        /// <summary>
        /// オブジェクトをdeltaTimeぶんだけ移動させる。
        /// </summary>
        public void Move(in Vector3 velocity)
        {
            _transform.position += velocity * _blackBoard.PausableDeltaTime;
        }

        /// <summary>
        /// 動的オフセットをdeltaTimeぶんだけ移動させる。
        /// </summary>
        public void OffsetMove(in Vector3 velocity)
        {
            _offset.position += velocity * _blackBoard.PausableDeltaTime;
        }

        /// <summary>
        /// 前方向を変更する。
        /// </summary>
        public void Forward(in Vector3 forward)
        {
            _rotate.forward = forward;
        }
    }
}
