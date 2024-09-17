using Enemy.DebugUse;
using UnityEngine;
using Enemy.Boss;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;

namespace Enemy
{
    /// <summary>
    /// 近接攻撃の装備。
    /// 攻撃タイミングはアニメーションイベント任せ。
    /// </summary>
    public class MeleeEquipment : Equipment
    {
        [SerializeField] private SphereCollider _hitBox;
        [SerializeField] private Effect _attackEffect;
        [SerializeField] private int _damage = 1;

        private Transform _rotate;
        private AnimationEvent _animationEvent;
        private IOwnerTime _owner;

        private float Radius
        {
            get
            {
                if (_hitBox != null) return _hitBox.radius;
                else return 0;
            }
        }

        private void Awake()
        {
            _rotate = FindRotate();
            _animationEvent = GetComponentInChildren<AnimationEvent>();
            _hitBox.OnTriggerEnterAsObservable().Subscribe(OnDamageCollisionHit).AddTo(this);
        }

        private void Start()
        {
            // 雑魚敵とボスの両用。
            if (TryGetComponent(out EnemyController e)) _owner = e.BlackBoard;
            else if (TryGetComponent(out BossController b)) _owner = b.BlackBoard;

            DisableHitBox();
        }

        private void OnEnable()
        {
            _animationEvent.OnMeleeAttackStart += EnableHitBox;
            _animationEvent.OnMeleeAttackEnd += DisableHitBox;
        }

        private void OnDisable()
        {
            _animationEvent.OnMeleeAttackStart -= EnableHitBox;
            _animationEvent.OnMeleeAttackEnd -= DisableHitBox;
        }

        private void OnDrawGizmosSelected()
        {
            // 攻撃範囲。
            GizmosUtils.WireCircle(Origin(), Radius, Color.red);
        }

        // 判定の有効化
        private void EnableHitBox()
        {
            EnableHitBox(true);
            OnCollision();

            // 最後に攻撃したタイミングを更新。
            LastAttackTiming = Time.time;
        }

        // 判定の無効化
        private void DisableHitBox()
        {
            EnableHitBox(false);
        }

        // 判定の有効/無効を切り替え。
        private void EnableHitBox(bool value)
        {
            if (_hitBox != null) _hitBox.enabled = value;
        }

        // トリガーに接触した際の処理。
        private void OnDamageCollisionHit(Collider other)
        {
            // コライダーと同じオブジェクトにコンポーネントが付いている前提。
            if (other.TryGetComponent(out IDamageable dmg)) dmg.Damage(_damage);
            // 判定の瞬間の演出
            if (_attackEffect != null) _attackEffect.Play(_owner);
        }

        /// <summary>
        /// 座標が攻撃範囲内かを返す。
        /// </summary>
        public bool IsWithinRange(in Vector3 point)
        {
            return (Origin() - point).sqrMagnitude <= Radius * Radius;
        }

        /// <summary>
        /// 派生クラスで射撃する際に追加で呼ぶ処理。
        /// </summary>
        protected virtual void OnCollision() { }

        // 攻撃の基準となる座標を返す。
        private Vector3 Origin()
        {
            if (_rotate == null) return transform.position;

            if (_hitBox != null) return _hitBox.transform.position;
            else return transform.position;
        }
    }
}
