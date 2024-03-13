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
    }
}
