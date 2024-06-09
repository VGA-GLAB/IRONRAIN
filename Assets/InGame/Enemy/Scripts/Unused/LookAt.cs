using UnityEngine;

namespace Enemy.Unused
{
    /// <summary>
    /// Y軸方向に回転させ対象の方を向く。
    /// </summary>
    public class LookAt : MonoBehaviour
    {
        [SerializeField] private Transform _axis;
        [SerializeField] private Transform _player;

        private void Update()
        {
            if (_axis == null || _player == null) return;

            // Y軸の差は無視
            Vector3 s = _axis.position;
            s.y = 0;
            Vector3 t = _player.position;
            t.y = 0;

            Vector3 dir = t - s;
            _axis.forward = Vector3.Lerp(_axis.forward, dir, Time.deltaTime);
        }
    }
}
