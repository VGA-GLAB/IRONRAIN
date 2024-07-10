using Enemy.Control.Boss;
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
    public class RangeEquipment : Equipment
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
        [Header("AimModeがTargetの場合")]
        [SerializeField] private Transform _target;

        private Transform _player;

        private IOwnerTime _owner;

        [Inject]
        private void Construct(Transform player)
        {
            _player = player;
        }

        private void Start()
        {
            // 雑魚敵とボスの両用。
            if (TryGetComponent(out EnemyController e)) _owner = e.BlackBoard;
            else if (TryGetComponent(out BossController b)) _owner = b.BlackBoard;
        }

        private void OnEnable()
        {
            //_animationEvent.OnRangeFireStart += Shoot;
        }

        private void OnDisable()
        {
            //_animationEvent.OnRangeFireStart -= Shoot;
        }

        // 弾を発射する。
        // アニメーションイベントに登録して呼んでもらう。
        private void Shoot()
        {
            if (_muzzle == null) return;
            if (_owner == null) { Debug.LogWarning("弾を発射する前に装備者への参照が必要。"); return; }

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

            // タイミングを更新。
            LastAttackTiming = Time.time;

            OnShoot();

            // 前方に撃つ
            void FireToForward()
            {
                BulletPool.Fire(_owner, _key, _muzzle.position, _muzzle.forward);
            }

            // 目標に向けて撃つ
            void FireToTarget(Transform target)
            {
                Vector3 f = (target.position - _muzzle.position).normalized;
                BulletPool.Fire(_owner, _key, _muzzle.position, f);
            }
        }

        /// <summary>
        /// 派生クラスで射撃する際に追加で呼ぶ処理。
        /// </summary>
        protected virtual void OnShoot() { }

        private void OnDrawGizmos()
        {
            if (_muzzle != null)
            {
                // 弾道。
                Vector3 f = _muzzle.position + _muzzle.forward * 10.0f; // 適当な長さ
                GizmosUtils.Line(_muzzle.position, f, ColorExtensions.ThinRed);
            }
        }
    }
}
