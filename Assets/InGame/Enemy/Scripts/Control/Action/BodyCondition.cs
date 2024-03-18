using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 身体の状態を変更する。
    /// </summary>
    public class BodyCondition
    {
        private Transform _transform;

        public BodyCondition(Transform transform)
        {
            _transform = transform;
        }

        /// <summary>
        /// 撃破されて非表示になる。
        /// </summary>
        public void Disable()
        {
            _transform.gameObject.SetActive(false);
        }
    }
}
