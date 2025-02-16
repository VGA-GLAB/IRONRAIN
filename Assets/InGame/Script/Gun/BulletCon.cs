using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Enemy;
using UnityEngine.Serialization;

namespace IronRain.Player
{
    public class BulletCon : MonoBehaviour
    {
        public event Action<BulletCon, PoolObjType> OnBulletRelease;
        public PlayerWeaponType WeaponType => _weaponType;

        [SerializeField] private float _speed;
        [SerializeField] private float _bossBattleSpeed;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private ParticleSystem[] _particleArray;
        [SerializeField] private VfxEffect _effect;
        [SerializeField] private SphereCollider _sphereCollider;
        [SerializeField] private Vector3 _offset;
        [Tooltip("コライダーを適用しておく時間")]
        [SerializeField] private float _useColliderTime = 0.7f;
        [Tooltip("ロックオンしている敵")]
        private GameObject _lockOnEnemy;

        private Vector3 _shotDir;
        private Transform _shootingTarget;
        private int _damege;
        private PlayerWeaponType _weaponType;
        private ObjectPool _pool;
        private bool _isBossBattle;
        private float _horizontalDot; // トリガーされたときの銃弾とエネミーの座標の内積

        public void SetUp(GameObject enemy, int damege, Vector3 shotDir, PlayerWeaponType weaponType, ObjectPool pool, bool isBossBattle)
        {
            _pool = pool;
            _lockOnEnemy = enemy;
            _damege = damege;
            _shotDir = shotDir;
            _weaponType = weaponType;
            _isBossBattle = isBossBattle;

            if (_lockOnEnemy && _lockOnEnemy.GetComponent<Enemy.Character>().TryFindShootingTarget(out Transform shootingTarget))
            {
                _shootingTarget = shootingTarget;
            }

            if (_lockOnEnemy != null)
            {
                Vector3 toEnemy = _lockOnEnemy.transform.position - transform.position;
                _horizontalDot = Vector3.Dot(transform.right, toEnemy.normalized); 
            }
        }

        private void Start()
        {
            if (_effect != null) 
            {
                _effect.Play();
            }
            
            if (!_shootingTarget)
            {
                //Debug.LogError($"shootingTargetがNullです");
            }
        }
        private void Update()
        {
            ///一旦完全追従に
            if (_shootingTarget && _lockOnEnemy && _lockOnEnemy.activeSelf )
            {
                transform.LookAt(_shootingTarget.position + _offset);
                _rb.velocity = transform.forward * GetSpeed() * ProvidePlayerInformation.TimeScale;
            }
            else if (_shootingTarget && _lockOnEnemy && !_lockOnEnemy.activeSelf)
            {
                _rb.velocity = _shotDir * GetSpeed() * ProvidePlayerInformation.TimeScale;
            }
            else
            {
                _rb.velocity = _shotDir * GetSpeed() * ProvidePlayerInformation.TimeScale;
            }
        }

        private async void OnTriggerEnter(Collider other)
        {
            var damageble = other.GetComponentInParent<IDamageable>();
            var playerCon = other.GetComponentInParent<PlayerController>();
            if (!playerCon && damageble != null)
            {
                damageble.Damage(_damege, _weaponType.ToString());
                var effect = _pool.GetEffect(PoolObjType.AssaultRifleImpactEffect);
                effect.gameObject.transform.position = other.ClosestPoint(this.transform.position);
                effect.SetUp(_pool);
                StopAllCoroutines();
                _sphereCollider.enabled = true; //コライダーを戻しておく
                _pool.ReleaseBullet(this);
            }
        }

        public void SetVisible(bool isVisible)
        {
            if (isVisible)
            {
                gameObject.SetActive(true);
                for (int i = 0; i < _particleArray.Length; i++)
                {
                    _particleArray[i].Play();
                }
                StartCoroutine(BulletRelese());
            }
            else
            {
                for (int i = 0; i < _particleArray.Length; i++)
                {
                    _particleArray[i].Stop();
                }
                StopAllCoroutines();
                gameObject.SetActive(false);
            }

            _sphereCollider.enabled = isVisible;
        }

        private float GetSpeed()
        {
            if (_isBossBattle)
            {
                return _bossBattleSpeed;
            }
            else
            {
                return _speed;
            }
        }

        private IEnumerator BulletRelese()
        {
            yield return new WaitForSeconds(_useColliderTime);

            _sphereCollider.enabled = false; //一定時間経過でコライダーを消す（未登場の敵に当たらないように）
            
            yield return new WaitForSeconds(2 - _useColliderTime);
            if (enabled)
            {
                _sphereCollider.enabled = true; //コライダーを戻しておく
                _pool.ReleaseBullet(this);
            }
        }
    }
}
