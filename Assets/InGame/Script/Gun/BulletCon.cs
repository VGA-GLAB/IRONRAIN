using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCon : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody _rb;

    private void Update()
    {
        _rb.AddForce(transform.forward * _speed, ForceMode.Impulse);
    }
}
