using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// オブジェクトを動かす。
    /// </summary>
    public class BodyMove
    {
        private Transform _transform;

        public BodyMove(Transform transform)
        {
            _transform = transform;
        }

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 TransformPosition => _transform.position;

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
            _transform.position += velocity * Time.deltaTime;
        }
    }
}
