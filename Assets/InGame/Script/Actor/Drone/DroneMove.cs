using System;
using UnityEngine;

/// <summary>ドローンの挙動</summary>
public class DroneMove : MonoBehaviour
{
    [SerializeField, Header("最大目標地点")] private Vector3 maxTargetPosition;
    [SerializeField, Header("最小目標地点")] private Vector3 minTargetPosition;
    [SerializeField, Header("タイプ")] private DroneType type;
    [SerializeField, Header("移動速度")] private float moveSpeed;
    
    private Vector3 _targetPosition;
    
    private enum DroneType
    {
        Horizontal, Vertical, Crossing
    }

    private void Start()
    {
        _targetPosition = maxTargetPosition;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Action moveAction = type switch
        {
            DroneType.Horizontal => HorizontalMove,
            DroneType.Vertical => VerticalMove,
            DroneType.Crossing => CrossingMove,
            _ => throw new NotSupportedException("Invalid Drone Type")
        };
        
        moveAction.Invoke();
    }

    private void HorizontalMove()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, _targetPosition, moveSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.localPosition, _targetPosition) < 0.01f)
        {
            _targetPosition = (_targetPosition == maxTargetPosition) ? minTargetPosition : maxTargetPosition;
        }
    }

    private void VerticalMove()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, _targetPosition, moveSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.localPosition, _targetPosition) < 0.01f)
        {
            _targetPosition = (_targetPosition == maxTargetPosition) ? minTargetPosition : maxTargetPosition;
        }
    }

    private void CrossingMove()
    {
        
    }
}
