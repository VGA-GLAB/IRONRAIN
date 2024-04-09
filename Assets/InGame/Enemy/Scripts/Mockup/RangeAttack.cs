using Enemy.Control;
using UnityEngine;

namespace Enemy.Mockup
{
    /// <summary>
    /// ダメージ判定のある弾を飛ばす遠距離攻撃
    /// </summary>
    public class RangeAttack : MonoBehaviour, IAttack
    {
        [Header("飛ばす弾の設定")]
        [SerializeField] private Transform _muzzle;
        [SerializeField] private Transform _forward;
        [SerializeField] private BulletKey _key;
        [Header("目標に向けて飛ばす場合")]
        [SerializeField] private Transform _target;
        [SerializeField] bool _isTargeting;

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
            else if (_forward != null)
            {
                BulletPool.Fire(_key, _muzzle.position, _forward.forward);
            }
        }
    }
}
