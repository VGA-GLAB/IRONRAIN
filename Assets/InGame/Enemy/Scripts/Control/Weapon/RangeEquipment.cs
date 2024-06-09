using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;
using VContainer;

namespace Enemy.Control
{
    /// <summary>
    /// 遠距離攻撃の装備。
    /// 装備者への参照を渡してもらい、発射自体はアニメーションイベントにフック。
    /// </summary>
    public class RangeEquipment : MonoBehaviour, IEquipment
    {
        enum AimMode
        {
            Forward, // マズルから真っ直ぐ。
            Player,  // プレイヤーに向ける。
            Target,  // 任意のターゲットに向ける。
        }

        [Header("アニメーションイベントに処理をフック")]
        [SerializeField] private AnimationEvent _animationEvent;
        [Header("射撃方法")]
        [SerializeField] private AimMode _aimMode;
        [Header("飛ばす弾の設定")]
        [SerializeField] private Transform _muzzle;
        [SerializeField] private BulletKey _key;
        [Header("目標に向けて飛ばす場合")]
        [SerializeField] private Transform _target;

        private Transform _player;
        private IOwnerTime _ownerTime;

        [Inject]
        private void Construct(Transform player)
        {
            _player = player;
        }

        private void OnEnable()
        {
            _animationEvent.OnFireStart += Shoot;
        }

        private void OnDisable()
        {
            _animationEvent.OnFireStart -= Shoot;
        }

        // 発射する前に装備者への参照が必要。
        void IEquipment.RegisterOwner(IOwnerTime ownerTime)
        {
            _ownerTime = ownerTime;
        }

        // 弾を発射する。
        // アニメーションイベントに登録して呼んでもらう。
        private void Shoot()
        {
            if (_muzzle == null) return;
            if (_ownerTime == null) { Debug.LogWarning("弾を発射する前に装備者への参照が必要。"); return; }

            // 射撃モード
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
                BulletPool.Fire(_ownerTime, _key, _muzzle.position, _muzzle.forward);
            }

            // 目標に向けて撃つ
            void FireToTarget(Transform target)
            {
                Vector3 f = (target.position - _muzzle.position).normalized;
                BulletPool.Fire(_ownerTime,_key, _muzzle.position, f);
            }
        }

        private void OnDrawGizmos()
        {
            // 弾道。
            Vector3 f = _muzzle.position + _muzzle.forward * 10.0f; // 適当な長さ
            GizmosUtils.Line(_muzzle.position, f, ColorExtensions.ThinRed);
        }
    }
}
