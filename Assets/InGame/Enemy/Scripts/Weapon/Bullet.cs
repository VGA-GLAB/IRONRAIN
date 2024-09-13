using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// プールから取り出され発射される。
    /// 非アクティブなった際にプールに戻る。
    /// </summary>
    public abstract class Bullet : MonoBehaviour
    {
        // ダメージの種類
        private enum DamageType { Assault, Launcher, Funnel }

        // ヒット後、プールに戻るまでのディレイ。
        const float ExplosionDelay = 1.0f;

        [SerializeField] private Transform _forward;
        [SerializeField] private Collider _trigger;
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private Effect _trailEffect;
        [SerializeField] private Effect _explosionEffect;
        [SerializeField] private DamageType _damageType;

        [Header("パラメータ設定")]
        [SerializeField] protected float _lifeTime = 1.0f;
        [SerializeField] protected float _speed = 10.0f;
        [SerializeField] private int _damage = 1;

        protected Transform _transform;
        private IOwnerTime _ownerTime;
        private float _elapsed;
        // 1フレーム前の位置と比較して前向きを修正する。
        private Vector3 _prev;

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
            // 前向きを修正。
            Vector3 current = _transform.position;

            Vector3 dir = _prev - current;
            if (dir != Vector3.zero) _forward.forward = dir;

            _prev = current;

            float deltaTime = _ownerTime != null ? _ownerTime.PausableDeltaTime : Time.deltaTime;
            _elapsed += deltaTime;

            if (_elapsed > _lifeTime + ExplosionDelay) ReturnToPool();
            else if (_elapsed > _lifeTime) StayExplosion();
            else StayShooting(deltaTime);
        }

        /// <summary>
        /// 飛ばす方向を指定して弾を撃つ。
        /// </summary>
        public void Shoot(Vector3 direction, IOwnerTime ownerTime)
        {
            View(ownerTime);
            OnShoot(direction);
        }

        /// <summary>
        /// 対象を指定して弾を撃つ。
        /// </summary>
        public void Shoot(Transform target, IOwnerTime ownerTime)
        {
            View(ownerTime);
            OnShoot(target);
        }

        // 発射時と飛翔中の処理は、継承先でオーバーライドする。
        protected virtual void OnShoot(Vector3 direction) { }
        protected virtual void OnShoot(Transform target) { }
        protected virtual void StayShooting(float deltaTime) { }

        // 画面に表示。
        private void View(IOwnerTime ownerTime)
        {
            RendererEnable(true);
            _ownerTime = ownerTime;
            _elapsed = 0;

            TrailEffect(true);
        }

        // ヒットした瞬間
        private void OnHit(Collider collider)
        {
            // 敵とNPCはコライダーとスクリプトが同じオブジェクトに無いので弾かれる。
            if (collider.TryGetComponent(out IDamageable damageable))
            {
                string weapon = string.Empty;
                if (_damageType == DamageType.Assault) weapon = Const.RifleWeaponName;
                else if (_damageType == DamageType.Launcher) weapon = Const.LauncherWeaponName;
                else if (_damageType == DamageType.Funnel) weapon = Const.FunnelWeaponName;

                damageable.Damage(_damage, weapon);
            }
            else return;

            // 経過時間を生存時間に上書きすることで、次のUpdateのタイミングで爆発に条件分岐する。
            _elapsed = _lifeTime;
        }

        // 弾がヒットした瞬間にオブジェクトを無効化すると、子になっているエフェクトが消えるので、
        // Rendererを無効化してディレイ後にオブジェクトを無効化する必要がある。
        private void StayExplosion()
        {
            RendererEnable(false);
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

        // 表示/非表示切り替え
        private void RendererEnable(bool value)
        {
            foreach (Renderer r in _renderers) r.enabled = value;
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