using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [Header("初速")]
    [SerializeField] private float _initialSpeed;

    private Vector3 _velocity;
    private Transform _target;

    private void Update()
    {
        transform.position += _velocity;
    }

    /// <summary>
    /// 発射後、目標を追尾し必ずヒットする。
    /// </summary>
    /// <param name="target">目標</param>
    /// <param name="launch">発射方向</param>
    public void Fire(Transform target, Vector3 launch)
    {
        launch = launch.normalized;
        
        _velocity = launch * _initialSpeed;
        _target = target;
    }
}