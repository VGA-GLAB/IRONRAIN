using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;

namespace Enemy.Mockup
{
    /// <summary>
    /// 当たり判定のレイキャストを用いた近距離攻撃
    /// </summary>
    public class MeleeAttack : MonoBehaviour
    {
        [Header("範囲の設定")]
        [SerializeField] private Transform _forward;
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
            if (_forward == null) return transform.position;

            // Y軸以外で回転しても正常な値を返す
            Vector3 f = _forward.forward * _forwardOffset;
            Vector3 h = _forward.up * _heightOffset;

            return transform.position + f + h;
        }

        private void OnDrawGizmos()
        {
            GizmosUtils.WireSphere(Origin(), _radius, new Color(1, 0, 0, 0.1f));
        }
    }
}
