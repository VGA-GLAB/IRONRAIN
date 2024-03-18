using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 近距離攻撃
    /// </summary>
    public class MeleeWeapon : MonoBehaviour, IWeapon
    {
        [Header("向きの基準")]
        [SerializeField] private Transform _rotate;
        [Header("範囲の設定")]
        [SerializeField] private float _forwardOffset;
        [SerializeField] private float _heightOffset;
        [SerializeField] private float _radius = 3.0f;

        /// <summary>
        /// 球状の当たり判定を出して攻撃
        /// </summary>
        public void Attack()
        {
            // 球状の当たり判定なので対象が上下にズレている場合は当たらない場合がある。
            RaycastExtensions.OverlapSphere(Origin(), _radius, col =>
            {
                // ダメージ用のインターフェースなどで判定
            });
        }

        // 攻撃の基準となる座標を返す
        private Vector3 Origin()
        {
            if (_rotate == null) return transform.position;

            // Y軸以外で回転しても正常な値を返す
            Vector3 f = _rotate.forward * _forwardOffset;
            Vector3 h = _rotate.up * _heightOffset;

            return transform.position + f + h;
        }

        private void OnDrawGizmos()
        {
            GizmosUtils.WireSphere(Origin(), _radius, ColorExtensions.ThinRed);
        }
    }
}
