using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>ドローンの挙動</summary>
public class DroneMove : MonoBehaviour
{
    [SerializeField, Header("X座標の最大値")] private float maxValueX;
    [SerializeField, Header("X座標の最小値")] private float minValueX;
    [SerializeField, Header("Y座標の最大値")] private float maxValueY;
    [SerializeField, Header("Y座標の最小値")] private float minValueY;
    [SerializeField, Header("タイプ")] private DroneType type;
    [SerializeField, Header("移動速度")] private float moveSpeed;
    
    private Vector3 _targetPosition;
    
    private enum DroneType
    {
        Random, Crossing, Turning
    }

    private void Start()
    {
        _targetPosition = GetRandomTargetPosition();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Action moveAction = type switch
        {
            DroneType.Random => RandomMove,
            DroneType.Crossing => CrossingMove,
            DroneType.Turning => TurningMove,
            _ => throw new NotSupportedException("Invalid Drone Type")
        };
        
        moveAction.Invoke();
    }

    private void RandomMove()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, _targetPosition, moveSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.localPosition, _targetPosition) < 0.01f)
        {
            _targetPosition = GetRandomTargetPosition();
        }
    }

    private void CrossingMove()
    {
        
    }

    private void TurningMove()
    {
        
    }

    private Vector3 GetRandomTargetPosition()
    {
        return new Vector3(Random.Range(minValueX, maxValueX), Random.Range(minValueY, maxValueY), 0);
    }
}
