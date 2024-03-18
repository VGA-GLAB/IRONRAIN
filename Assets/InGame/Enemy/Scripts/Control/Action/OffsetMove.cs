using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 動的オフセットを移動させる。
    /// </summary>
    public class OffsetMove
    {
        private Transform _offset;

        public OffsetMove(Transform offset)
        {
            _offset = offset;
        }
    }
}
