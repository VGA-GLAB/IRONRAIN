using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPanel : MonoBehaviour
{
    [SerializeField, Tooltip("追従させたいオブジェクト")]
    private Transform _playerTransform;
    /// <summary>
    /// プレイヤーとの初期位置のオフセット
    /// </summary>
    private Vector3 _initialOffset;
    
    /// <summary>
    /// プレイヤーとの初期回転
    /// </summary>
    private Quaternion _initialRotation;
    
    void Awake()
    {
        // プレイヤーとの初期位置と回転を計算
        _initialOffset = transform.position - _playerTransform.position;
        _initialRotation = Quaternion.Inverse(_playerTransform.rotation) * transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //回転後に位置を調整する
        transform.position = _playerTransform.position + _initialOffset;
        Vector3 target = transform.position;
        target.z += 10;
        transform.LookAt(target);
    }
}
