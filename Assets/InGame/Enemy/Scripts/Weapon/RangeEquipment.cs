using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;
using Enemy.Boss;
using UnityEngine.Events;

namespace Enemy
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

        /// <summary>
        /// 弾を発射するタイミングのコールバック。
        /// </summary>
        public event UnityAction OnShootAction;

        [Header("エフェクトの設定")]
        [SerializeField] private Effect[] _muzzleEffects;
        [Header("射撃方法")]
        [SerializeField] private AimMode _aimMode;
        [Header("飛ばす弾の設定")]
        [SerializeField] private Transform _muzzle;
        [SerializeField] private BulletKey _key;
        [Header("AimModeがTargetの場合")]
        [SerializeField] private Transform _target;

        private Transform _player;
        private Transform _rotate;
        private AnimationEvent _animationEvent;
        private IOwnerTime _owner;

        private void Awake()
        {
            _player = FindPlayer();
            _rotate = FindRotate();
            // Animatorが1つだけの前提。
            _animationEvent = GetComponentInChildren<AnimationEvent>();
        }

        private void Start()
        {
            // マズルが設定されていない場合は自身から発射する。
            if (_muzzle == null) _muzzle = transform;

            // 雑魚敵とボスの両用。
            if (TryGetComponent(out EnemyController e)) _owner = e.BlackBoard;
            else if (TryGetComponent(out BossController b)) _owner = b.BlackBoard;
        }

        private void OnEnable()
        {
            _animationEvent.OnRangeFireStart += Shoot;
        }

        private void OnDisable()
        {
            _animationEvent.OnRangeFireStart -= Shoot;
        }

        // 弾を発射する。
        // アニメーションイベントに登録して呼んでもらう。
        private void Shoot()
        {
            if (_owner == null) { Debug.LogWarning("弾を発射する前に装備者への参照が必要。"); return; }

            // 射撃モード
            switch (_aimMode)
            {
                case AimMode.Player when _key == BulletKey.Assault:
                    LerpFireToTarget(_player); break;
                case AimMode.Player when _key == BulletKey.Launcher:
                    LerpFireToTarget(_player); break;
                case AimMode.Forward: 
                    Fire(_rotate.forward); break;
                case AimMode.Player: 
                    FireToTarget(_player); break;
                case AimMode.Target: 
                    FireToTarget(_target); break;
            }

            // タイミングを更新。
            LastAttackTiming = Time.time;

            // マズルのエフェクトを再生
            if (_muzzleEffects != null)
            {
                foreach (Effect e in _muzzleEffects) e.Play(_owner);
            }

            OnShoot();
            OnShootAction?.Invoke();
        }

        // 弾をプールから取り出して任意の方向に発射。
        private void Fire(in Vector3 dir)
        {
            BulletPool.Fire(_owner, _key, _muzzle.position, dir);
        }

        // 任意の目標に向けて発射。
        private void FireToTarget(Transform target)
        {
            // 射撃中に目標が死亡などでnullになった場合は正面に射撃しておく。
            if (target == null) { Fire(_muzzle.forward); return; }

            Vector3 dir = (target.position - _muzzle.position).normalized;
            Fire(dir);
        }

        // 任意の目標に向けて発射。
        private void LerpFireToTarget(Transform target)
        {
            BulletPool.Fire(_owner, _key, _muzzle.position, target);
        }

        /// <summary>
        /// 派生クラスで射撃する際に追加で呼ぶ処理。
        /// </summary>
        protected virtual void OnShoot() { }

        private void OnDrawGizmosSelected()
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
