using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;
using VContainer;

namespace Enemy.Control
{
    /// <summary>
    /// 遠距離攻撃の装備
    /// </summary>
    public class RangeEquipment : MonoBehaviour, IEquipment
    {
        enum AimMode
        {
            Forward, // マズルから真っ直ぐ。
            Player,  // プレイヤーに向ける。
            Target,  // 任意のターゲットに向ける。
        }

        [Header("射撃方法")]
        [SerializeField] private AimMode _aimMode;
        [Header("飛ばす弾の設定")]
        [SerializeField] private Transform _muzzle;
        [SerializeField] private BulletKey _key;
        [Header("目標に向けて飛ばす場合")]
        [SerializeField] private Transform _target;

        private Transform _player;
        // 毎フレーム攻撃のアニメーションをトリガーしないようにフラグで管理
        private bool _isAttackAnimationPlaying;

        [Inject]
        private void Construct(Transform player)
        {
            _player = player;
        }

        void IEquipment.Attack(IOwnerTime ownerTime)
        {
            if (_muzzle == null) return;

            switch (_aimMode)
            {
                case AimMode.Forward:
                    FireToForward();
                    break;
                case AimMode.Player:
                    FireToTarget(_player);
                    break;
                case AimMode.Target when _target != null:
                    FireToTarget(_target);
                    break;
            }

            // 前方に撃つ
            void FireToForward()
            {
                BulletPool.Fire(ownerTime, _key, _muzzle.position, _muzzle.forward);
            }

            // 目標に向けて撃つ
            void FireToTarget(Transform target)
            {
                Vector3 f = (target.position - _muzzle.position).normalized;
                BulletPool.Fire(ownerTime,_key, _muzzle.position, f);
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
