using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace IronRain.Player
{
    public class BulletCon : MonoBehaviour
    {
        public event Action<BulletCon> OnRelease;

        [SerializeField] private float _speed;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private ParticleSystem[] _particleArray;
        [SerializeField] private SphereCollider _sphereCollider;
        [SerializeField] private Vector3 _offset;
        [Tooltip("ロックオンしている敵")]
        private GameObject _lockOnEnemy;

        private Vector3 _shotDir;
        private Transform _shootingTarget;
        private int _damege;
        private string _weaponName;

        public void SetUp(GameObject enemy, int damege, Vector3 shotDir, string weaponName)
        {
            _lockOnEnemy = enemy;
            _damege = damege;
            _shotDir = shotDir;
            _weaponName = weaponName;

            if (_lockOnEnemy && _lockOnEnemy.GetComponent<Enemy.Character>().TryFindShootingTarget(out Transform shootingTarget))
            {
                _shootingTarget = shootingTarget;
            }

        }

        private void Start()
        {
            if (!_shootingTarget)
            {
                //Debug.LogError($"shootingTargetがNullです");
            }
        }
        private void Update()
        {
            ///一旦完全追従に
            if (_shootingTarget && _lockOnEnemy && _lockOnEnemy.activeSelf)
            {
                transform.LookAt(_shootingTarget.position + _offset);
                _rb.velocity = transform.forward * _speed * ProvidePlayerInformation.TimeScale;
            }
            else if (_shootingTarget && _lockOnEnemy && !_lockOnEnemy.activeSelf)
            {
                _rb.velocity = _shotDir * _speed * ProvidePlayerInformation.TimeScale;
            }
            else
            {
                _rb.velocity = _shotDir * _speed * ProvidePlayerInformation.TimeScale;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var damageble = other.GetComponentInParent<IDamageable>();
            var playerCon = other.GetComponentInParent<PlayerController>();
            if (!playerCon && damageble != null)
            {
                damageble.Damage(_damege, _weaponName);
                OnRelease?.Invoke(this);
            }
        }

        public void SetVisible(bool isVisible)
        {
            if (_weaponName != PlayerWeaponType.RocketLauncher.ToString())
                if (isVisible)
                {
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
                }

            _sphereCollider.enabled = isVisible;
        }

        private IEnumerator BulletRelese()
        {
            yield return new WaitForSeconds(2);
            OnRelease?.Invoke(this);
        }
    }
}
