using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{

    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed;
    void Start()
    {
        
    }

    void Update()
    {
        _rb.velocity = transform.forward * _speed;
    }
}
