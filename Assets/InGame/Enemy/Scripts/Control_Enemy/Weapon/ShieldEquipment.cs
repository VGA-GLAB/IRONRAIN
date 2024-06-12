using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 盾持ち敵の装備。
    /// 攻撃タイミングはアニメーションイベント任せ。
    /// </summary>
    public class ShieldEquipment : MonoBehaviour
    {
        [Header("アニメーションイベントに処理をフック")]
        [SerializeField] private AnimationEvent _animationEvent;
        [Header("向きの基準")]
        [SerializeField] private Transform _rotate;
        [Header("範囲の設定")]
        [SerializeField] private float _forwardOffset;
        [SerializeField] private float _heightOffset;
        [SerializeField] private float _radius = 3.0f;

        private void OnEnable()
        {
            _animationEvent.OnFireStart += Collision;
        }

        private void OnDisable()
        {
            _animationEvent.OnFireStart -= Collision;
        }

        // 当たり判定を出してダメージを与える。
        private void Collision()
        {
            // 球状の当たり判定なので対象が上下にズレている場合は当たらない場合がある。
            RaycastExtensions.OverlapSphere(Origin(), _radius, col =>
            {
                if (col == null) return;
                // コライダーと同じオブジェクトにコンポーネントが付いている前提。
                if (col.TryGetComponent(out IDamageable dmg)) dmg.Damage(1);
            });
        }

        // 攻撃の基準となる座標を返す。
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
            // 攻撃範囲。
            GizmosUtils.WireSphere(Origin(), _radius, ColorExtensions.ThinRed);
        }
    }
}
