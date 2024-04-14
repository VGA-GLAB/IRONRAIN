using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerMock : MonoBehaviour
{
    public Vector2 Dir => _dir;
    public MoveState MoveState => _moveState;

    [SerializeField] private LeverControllerMock _leftController;
    [SerializeField] private LeverControllerMock _rightController;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private PlayerSetting _playerSetting;

    [SerializeField] private float _oneGearSpeed;
    [SerializeField] private float _twoGearSpeed;
    [SerializeField] private float _threeGearSpeed;
    [SerializeField] private int _rotateSpeed;

    private Vector2 _dir;
    private MoveState _moveState;

    public void Setup() 
    {
        
    }

    private void Update()
    {
        Move();
    }

    //ToDo;操作の仕様が出来次第別クラスに分離する
    private void Move()
    {
        _dir = new Vector2(_leftController.ControllerDir.x, _rightController.ControllerDir.x);

        //前進
        if (_leftController.ControllerDir.x == 1 && _rightController.ControllerDir.x == 1)
        {
            _rb.velocity = transform.forward * _threeGearSpeed;
            _moveState = MoveState.Forward;
        }
        //後退
        else if (_leftController.ControllerDir.x == -1 && _rightController.ControllerDir.x == -1)
        {
            _rb.velocity = transform.forward * _oneGearSpeed;
            _moveState = MoveState.Back;
        }
        //左旋回
        else if (_leftController.ControllerDir.x == 1 && _rightController.ControllerDir.x != 1)
        {
            transform.Rotate(0, _rotateSpeed, 0);
            _moveState = MoveState.Left;
        }
        //右旋回
        else if (_leftController.ControllerDir.x != 1 && _rightController.ControllerDir.x == 1)
        {
            transform.Rotate(0, _rotateSpeed * -1, 0);
            _moveState = MoveState.Right;
        }
        else if (_leftController.ControllerDir.x == 0 && _rightController.ControllerDir.x == 0) 
        {
            _rb.velocity = transform.forward * _twoGearSpeed;
        }
    }
}

public enum MoveState
{
    Forward,
    Back,
    Right,
    Left,
}
