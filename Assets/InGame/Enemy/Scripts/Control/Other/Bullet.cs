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
        [SerializeField] private float _lifeTime = 1.0f;
        [SerializeField] private float _speed = 10.0f;

        private Transform _transform;
        private Vector3 _direction;
        private float _elapsed;

        private void Awake()
        {
            if (_trigger != null) _trigger.isTrigger = true;

            _transform = transform;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;

            if (_elapsed > _lifeTime)
            {
                gameObject.SetActive(false);
            }
            else
            {
                _transform.position += _direction * Time.deltaTime * _speed;
            }
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
