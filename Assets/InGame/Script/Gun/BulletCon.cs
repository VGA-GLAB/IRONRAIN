using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BulletCon : MonoBehaviour
{
    public event Action<BulletCon> OnRelease;

    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody _rb;
    [Tooltip("ロックオンしている敵")]
    private GameObject _lockOnEnemy;

    private Vector3 _shotDir;
    private int _damege;

    public void SetUp(GameObject enemy, int damege, Vector3 shotDir)
    {
        _lockOnEnemy = enemy;
        _damege = damege;
        _shotDir = shotDir;
    }
    private void Update()
    {
        ///一旦完全追従に
        if (_lockOnEnemy && _lockOnEnemy.activeSelf)
        {
            transform.LookAt(_lockOnEnemy.transform);
            _rb.velocity = transform.forward * _speed * ProvidePlayerInformation.TimeScale;
        }
        else if (_lockOnEnemy && !_lockOnEnemy.activeSelf) 
        {
            _rb.velocity = _shotDir * _speed * ProvidePlayerInformation.TimeScale;
        }
        else
        {
            _rb.velocity = _shotDir * _speed * ProvidePlayerInformation.TimeScale;
        }
    }

    private void OnEnable()
    {
          StartCoroutine(BulletRelese());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        var damageble = other.GetComponentInParent<IDamageable>();  
        var playerCon = other.GetComponentInParent<PlayerController>();
        if (!playerCon && damageble != null) 
        {                                                                                                                                                                                                   
            damageble.Damage(_damege);
            gameObject.SetActive(false);
            OnRelease?.Invoke(this);
        }
    }

    private IEnumerator BulletRelese() 
    {
        yield return new WaitForSeconds(2);
        OnRelease?.Invoke(this);
    }
}
