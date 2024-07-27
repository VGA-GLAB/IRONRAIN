using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// プールから取り出され発射される。
    /// 非アクティブなった際にプールに戻る。
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        // ヒット後、プールに戻るまでのディレイ。
        const float ExplosionDelay = 1.0f;

        [SerializeField] private Collider _trigger;
        [SerializeField] private Renderer _renderer;
        [Header("エフェクトの設定")]
        [SerializeField] private Effect _trailEffect;
        [SerializeField] private Effect _explosionEffect;
        [Header("パラメータ設定")]
        [SerializeField] private float _lifeTime = 1.0f;
        [SerializeField] private float _speed = 10.0f;
        [SerializeField] private int _damage = 1;

        private IOwnerTime _ownerTime;
        private Transform _transform;
        private Vector3 _direction;
        private float _elapsed;

        private void Awake()
        {
            if (_trigger != null)
            {
                _trigger.isTrigger = true;
                _trigger.OnTriggerEnterAsObservable().Subscribe(OnHit).AddTo(this);
            }

            _transform = transform;
        }
        
        private void Update()
        {
            float deltaTime = _ownerTime != null ? _ownerTime.PausableDeltaTime : Time.deltaTime;
            _elapsed += deltaTime;

            if (_elapsed > _lifeTime + ExplosionDelay) ReturnToPool();
            else if (_elapsed > _lifeTime) StayExplosion();
            else StayShooting(deltaTime);
        }

        /// <summary>
        /// 弾を撃ちだす。
        /// 一定時間経過で非アクティブになる。
        /// </summary>
        public void Shoot(Vector3 direction, IOwnerTime ownerTime)
        {
            _renderer.enabled = true;
            _direction = direction.normalized;
            _ownerTime = ownerTime;
            _elapsed = 0;

            TrailEffect(true);
        }

        // ヒットした瞬間
        private void OnHit(Collider collider)
        {
            if (collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(_damage);
            }

            // 経過時間を生存時間に上書きすることで、次のUpdateのタイミングで爆発に条件分岐する。
            _elapsed = _lifeTime;
        }

        // 発射後、飛翔中
        private void StayShooting(float deltaTime)
        {
            _transform.position += _direction * deltaTime * _speed;
        }

        // 弾がヒットした瞬間にオブジェクトを無効化すると、子になっているエフェクトが消えるので、
        // Rendererを無効化してディレイ後にオブジェクトを無効化する必要がある。
        private void StayExplosion()
        {
            _renderer.enabled = false;
            TrailEffect(false);
            ExplosionEffect(true);
        }

        // プールに戻す。
        private void ReturnToPool()
        {
            _ownerTime = null;
            gameObject.SetActive(false);
            ExplosionEffect(false);
        }

        // トレイルエフェクト
        private void TrailEffect(bool value)
        {
            if (_trailEffect == null) return;

            if (value) _trailEffect.Play(_ownerTime);
            else _trailEffect.Stop(); 
        }

        // 爆発エフェクト
        private void ExplosionEffect(bool value)
        {
            if (_explosionEffect == null) return;

            if (value && !_explosionEffect.IsPlaying) _explosionEffect.Play(_ownerTime);
            else if (!value) _explosionEffect.Stop();
        }
    }
}