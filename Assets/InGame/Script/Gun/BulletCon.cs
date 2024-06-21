using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCon : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody _rb;
    [Tooltip("ロックオンしている敵")]
    private GameObject _lockOnEnemy;

    private Vector3 _shotDir;
    private int _damege;

    public void SetUp(GameObject enemy, int damege, Vector3 shotDir)
    {
        _lockOnEnemy = enemy;
        //Debug.Log(_lockOnEnemy.name);
        _damege = damege;
        _shotDir = shotDir;
    }

    private void Start()
    {
        Destroy(this.gameObject, 5f);
    }

    private void Update()
    {
        ///一旦完全追従に
        if (_lockOnEnemy)
        {
            transform.LookAt(_lockOnEnemy.transform);
        }
        _rb.velocity = _shotDir * _speed * ProvidePlayerInformation.TimeScale;

    }

    private void OnTriggerEnter(Collider other)
    {
        var damageble = other.GetComponentInParent<IDamageable>();  
        var playerCon = other.GetComponentInParent<PlayerController>();
        if (!playerCon && damageble != null) 
        {                                                                                                                                                                                                   
            damageble.Damage(_damege);
            Destroy(this.gameObject);
        }
    }
}
