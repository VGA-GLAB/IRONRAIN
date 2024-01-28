using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField] private LeverController _leftController;
    [SerializeField] private LeverController _rightController;
    [SerializeField] private Rigidbody _rb;

   [SerializeField] private int _moveSpeed;
   [SerializeField] private int _rotateSpeed;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        //ëOêi
        if (_leftController.ControllerDir.x == 1 && _rightController.ControllerDir.x == 1)
        {
            _rb.velocity = new Vector3(0, 0, _moveSpeed);
        }
        //å„ëﬁ
        else if (_leftController.ControllerDir.x == -1 && _rightController.ControllerDir.x == -1)
        {
            _rb.velocity = new Vector3(0, 0, _moveSpeed * -1);
        }
        //ç∂ê˘âÒ
        else if (_leftController.ControllerDir.x == 1 && _rightController.ControllerDir.x == 0)
        {
            transform.Rotate(0, 0, _rotateSpeed);
        }
        //âEê˘âÒ
        else if (_leftController.ControllerDir.x == 0 && _rightController.ControllerDir.x == 1) 
        {
            transform.Rotate(0, 0, _rotateSpeed * -1);
        }
    }
}
