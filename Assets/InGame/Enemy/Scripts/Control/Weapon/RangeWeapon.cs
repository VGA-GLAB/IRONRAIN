using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 遠距離攻撃
    /// </summary>
    public class RangeWeapon : MonoBehaviour, IWeapon
    {
        [Header("飛ばす弾の設定")]
        [SerializeField] private Transform _muzzle;
        [SerializeField] private BulletKey _key;
        [Header("目標に向けて飛ばす場合")]
        [SerializeField] private Transform _target;
        [SerializeField] private bool _isTargeting;

        /// <summary>
        /// 弾を発射して攻撃
        /// </summary>
        public void Attack()
        {
            if (_muzzle == null) return;

            if (_isTargeting && _target != null)
            {
                Vector3 f = (_target.position - _muzzle.position).normalized;
                BulletPool.Fire(_key, _muzzle.position, f);
            }
            else if (_muzzle != null)
            {
                BulletPool.Fire(_key, _muzzle.position, _muzzle.forward);
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 f = _muzzle.position + _muzzle.forward * 10.0f; // 適当な長さ
            GizmosUtils.Line(_muzzle.position, f, ColorExtensions.ThinRed);
        }
    }
}