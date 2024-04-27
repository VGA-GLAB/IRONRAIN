using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCon : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody _rb;
    [Tooltip("���b�N�I�����Ă���G")]
    private GameObject _lockOnEnemy;

    private int _damege;

    public void SetUp(GameObject enemy, int damege)
    {
        _lockOnEnemy = enemy;
        //Debug.Log(_lockOnEnemy.name);
        _damege = damege;
    }

    private void Update()
    {
        ///��U���S�Ǐ]��
        if (_lockOnEnemy)
        {
            transform.LookAt(_lockOnEnemy.transform);
        }
        _rb.velocity = transform.forward * _speed * ProvidePlayerInformation.TimeScale;

    }

    private void OnTriggerEnter(Collider other)
    {
        var damageble = other.GetComponentInParent<IDamageable>();  
        var playerCon = other.GetComponentInParent<PlayerController>();
        if (!playerCon && damageble != null) 
        {                                                                                                                                                                                                   
            damageble.Damage(_damege);
            Debug.Log($"{other.name}�Ƀ_���[�W��^����");
            Destroy(this.gameObject);
        }
    }
}
