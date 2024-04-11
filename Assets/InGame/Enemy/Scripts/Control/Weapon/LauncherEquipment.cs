using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// ランチャーの敵の装備
    /// </summary>
    public class LauncherEquipment : MonoBehaviour, IEquipment
    {
        [Header("飛ばす弾の設定")]
        [SerializeField] private Transform _muzzle;
        [SerializeField] private BulletKey _key;
        [Header("目標に向けて飛ばす場合")]
        [SerializeField] private Transform _target;
        [SerializeField] private bool _isTargeting;

        // 毎フレーム攻撃のアニメーションをトリガーしないようにフラグで管理
        private bool _isAttackAnimationPlaying;

        void IEquipment.Attack()
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

        private void OnDrawGizmos()
        {
            Vector3 f = _muzzle.position + _muzzle.forward * 10.0f; // 適当な長さ
            GizmosUtils.Line(_muzzle.position, f, ColorExtensions.ThinRed);
        }
    }
}