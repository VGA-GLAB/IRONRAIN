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
        [SerializeField] private Collider _trigger;
        [Header("パラメータ設定")]
        [SerializeField] private float _lifeTime = 1.0f;
        [SerializeField] private float _speed = 10.0f;
        [SerializeField] private int _damage = 1;

        private Transform _transform;
        private Vector3 _direction;
        private float _elapsed;

        private void Awake()
        {
            if (_trigger != null)
            {
                _trigger.isTrigger = true;
                _trigger.OnTriggerEnterAsObservable().Subscribe(Hit).AddTo(this);
            }

            _transform = transform;
        }
        
        private void Update()
        {
            // QTEでスローになるのを考慮
            float deltaTime = Time.deltaTime * ProvidePlayerInformation.TimeScale;

            _elapsed += deltaTime;

            if (_elapsed > _lifeTime)
            {
                CombatDesigner.FireReport(isHit: false);
                gameObject.SetActive(false);
            }
            else
            {
                _transform.position += _direction * deltaTime * _speed;
            }
        }

        // 弾の当たり判定
        private void Hit(Collider collider)
        {
            if (!collider.TryGetComponent(out IDamageable damageable)) return;

            damageable.Damage(_damage);
            CombatDesigner.FireReport(isHit: true);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 弾を撃ちだす。
        /// 一定時間経過で非アクティブになる。
        /// </summary>
        public void Shoot(Vector3 direction)
        {
            _direction = direction.normalized;
            _elapsed = 0;
        }
    }
}
