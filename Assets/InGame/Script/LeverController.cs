using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class LeverController : MonoBehaviour
{
    /// <summary>�R���g���[���̌����Ă������</summary>
    public Vector3 ControllerDir => _controllerDir;

    [SerializeField] private InputActionProperty _controller;
    [SerializeField] private XRGrabInteractable _xRGrabInteractable;
    [SerializeField] private Transform _rightController;
    [Header("�O���̊�ƂȂ郍�[�J����ԃx�N�g��")]
    [SerializeField] private Vector3 _forward = Vector3.forward;
    [Header("X�p�x����")]
    [SerializeField] private float _xMax, _xMin;
    [Header("Z�p�x����")]
    [SerializeField] private float _zMax, _zMin;
    [SerializeField] private GameObject _bulletPrefab;
    [Header("�p�[�e�B�N��")]
    [SerializeField] private GameObject _particleEffect;
    [SerializeField] private Transform _bulletInsPos;
    [SerializeField] private float _rotateSpeed = 5;
    [SerializeField] private Transform _leverTransform;

    private Vector3 _controllerRotate = Vector3.zero;
    private Vector3 _controllerDir;
    private Vector3 _defaultRotate;
    private Vector3 _controllerSavePos;
    private bool _isLeverMove = false;
    private bool _isShot;

    void Start()
    {
        _defaultRotate = transform.localEulerAngles;
        _xRGrabInteractable.firstSelectEntered.AddListener(x => _isLeverMove = true);
        _xRGrabInteractable.lastSelectExited.AddListener(x => _isLeverMove = false);
    }

    void Update()
    {
        LeverRotate2();
        Shot();
    }

    /// <summary>
    /// ���o�[�̃��[�e�[�V�����𐧌䂷��
    /// </summary>
    private void LeverRotate()
    {
        if (_isLeverMove)
        {
            var dir = _rightController.position - transform.position;
            // �^�[�Q�b�g�̕����ւ̉�]
            //var lookAtRotation = Quaternion.LookRotation(dir, Vector3.up);
            // ��]�␳
            //var offsetRotation = Quaternion.FromToRotation(_forward, Vector3.forward);

            transform.LookAt(_rightController);
            //transform.rotation = lookAtRotation * offsetRotation;

            // Debug.Log($"xMax�l:{_xMax}���݂�X{transform.localEulerAngles.x}����:{_xMax < transform.localEulerAngles.x}");

            //�p�x����
            if (_xMax < transform.localEulerAngles.x)
            {
                //var rotate = transform.localEulerAngles;
                //rotate.x = _xMax;
                //transform.localEulerAngles = rotate;
            }
            if (_xMin > transform.localEulerAngles.x)
            {
                //var rotate = transform.localEulerAngles;
                //rotate.x = _xMin;
                //transform.localEulerAngles = rotate;
            }
        }
    }

    /// <summary>
    /// ���o�[�̃��[�e�[�V�����𐧌䂷��Ă��̂Q
    /// </summary>
    private void LeverRotate2() 
    {
        if (_isLeverMove)
        {
            //���o�[����
            var dir = Anglelimit(_rightController.transform.position - _controllerSavePos);
            var moveRotate = new Vector3(dir.z, 0, dir.x);
            Debug.Log($"{_controllerRotate} MoveRotate{moveRotate}");
            _controllerRotate += moveRotate * _rotateSpeed;
            transform.Rotate(moveRotate * _rotateSpeed);
        }
        //Y�����͎g��Ȃ��̂ŌŒ�
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, transform.localEulerAngles.z);
        SetControllerDir();
        _controllerSavePos = _rightController.transform.position;
    }

    /// <summary>
    /// �p�x��������֐�
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    private Vector3 Anglelimit(Vector3 dir) 
    {
        if (_xMax < _controllerRotate.x + (dir.z * _rotateSpeed)
            || _xMin > _controllerRotate.x + (dir.z * _rotateSpeed))
        {
            dir.z = 0;
        }

        if (_zMax < _controllerRotate.z + (dir.x * _rotateSpeed)
            || _zMin > _controllerRotate.z + (dir.x * _rotateSpeed)) 
        {
            dir.x = 0;
        }

        return dir;
    }

    /// <summary>
    /// �R���g���[���̌����Ă���������Z�b�g����
    /// </summary>
    private void SetControllerDir() 
    {
        if (_defaultRotate.x > transform.localEulerAngles.x)
        {
            _controllerDir.y = -1;
        }
        else if (_defaultRotate.x < transform.localEulerAngles.x) 
        {
            _controllerDir.y = 1;
        }

        if (_defaultRotate.z > transform.localEulerAngles.z)
        {
            _controllerDir.z = -1;
        }
        else if (_defaultRotate.z < transform.localEulerAngles.z) 
        {
            _controllerDir.z = 1;
        }
    }

    /// <summary>
    /// �e�𔭎˂���
    /// </summary>
    private void Shot() 
    {
        if (_xMax - 4 < transform.localEulerAngles.x && !_isShot)
        {
            Instantiate(_bulletPrefab, _bulletInsPos);
            //Instantiate(_particleEffect, _bulletInsPos);
            _isShot = true;
        }
        else if (_xMax - 4 > transform.localEulerAngles.x && _isShot)
        {
            _isShot = false;
        }

        if (Input.GetMouseButtonDown(0)) 
        {
            Instantiate(_bulletPrefab, _bulletInsPos);
            Instantiate(_particleEffect, _bulletInsPos);
            Debug.Log("�e��ł���");
        }
    }
}
