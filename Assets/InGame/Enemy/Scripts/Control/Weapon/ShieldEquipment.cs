using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 盾持ちの敵の装備
    /// </summary>
    public class ShieldEquipment : MonoBehaviour, IEquipment
    {
        [Header("向きの基準")]
        [SerializeField] private Transform _rotate;
        [Header("範囲の設定")]
        [SerializeField] private float _forwardOffset;
        [SerializeField] private float _heightOffset;
        [SerializeField] private float _radius = 3.0f;

        // 毎フレーム攻撃のアニメーションをトリガーしないようにフラグで管理
        private bool _isAttackAnimationPlaying;

        void IEquipment.Attack(IOwnerTime ownerTime)
        {
            // 球状の当たり判定なので対象が上下にズレている場合は当たらない場合がある。
            RaycastExtensions.OverlapSphere(Origin(), _radius, col =>
            {
                // ダメージ用のインターフェースなどで判定
            });
        }

        void IEquipment.PlayAttackAnimation(BodyAnimation animation)
        {
            if (_isAttackAnimationPlaying) return;

            _isAttackAnimationPlaying = true;
            animation.SetTrigger(Const.AnimationParam.AttackTrigger);
        }

        void IEquipment.PlayAttackEndAnimation(BodyAnimation animation)
        {
            animation.SetTrigger(Const.AnimationParam.AttackEndTrigger);
            _isAttackAnimationPlaying = false;
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
