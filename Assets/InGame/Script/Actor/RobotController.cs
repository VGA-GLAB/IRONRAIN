using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public Vector2 Dir => _dir;
    public MoveState MoveState => _moveState;

    [SerializeField] private LeverController _leftController;
    [SerializeField] private LeverController _rightController;
    [SerializeField] private Rigidbody _rb;

    [SerializeField] private float _oneGearSpeed;
    [SerializeField] private float _twoGearSpeed;
    [SerializeField] private float _threeGearSpeed;
    [SerializeField] private int _rotateSpeed;

    private Vector2 _dir;
    private MoveState _moveState;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        _dir = new Vector2(_leftController.ControllerDir.x, _rightController.ControllerDir.x);

        //ëOêi
        if (_leftController.ControllerDir.x == 1 && _rightController.ControllerDir.x == 1)
        {
            _rb.velocity = transform.forward * _threeGearSpeed;
            _moveState = MoveState.Forward;
        }
        //å„ëﬁ
        else if (_leftController.ControllerDir.x == -1 && _rightController.ControllerDir.x == -1)
        {
            _rb.velocity = transform.forward * _oneGearSpeed;
            _moveState = MoveState.Back;
        }
        //ç∂ê˘âÒ
        else if (_leftController.ControllerDir.x == 1 && _rightController.ControllerDir.x != 1)
        {
            transform.Rotate(0, _rotateSpeed, 0);
            _moveState = MoveState.Left;
        }
        //âEê˘âÒ
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
