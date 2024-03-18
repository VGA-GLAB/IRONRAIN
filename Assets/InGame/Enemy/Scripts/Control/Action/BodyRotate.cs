using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// オブジェクトをY軸で回転させる。
    /// </summary>
    public class BodyRotate
    {
        private Transform _rotate;

        public BodyRotate(Transform rotate)
        {
            _rotate = rotate;
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
