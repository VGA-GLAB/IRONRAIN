using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System;

public class LeverControllerMock : MonoBehaviour
{
    /// <summary>�R���g���[���̌����Ă������</summary>
    public Vector3 ControllerDir => _controllerDir;
    [SerializeField] private InputActionProperty _controllerTriggerInput;
    [SerializeField] private Transform _handTransfrom;
    [Header("�O���̊�ƂȂ郍�[�J����ԃx�N�g��")]
    [SerializeField] private Vector3 _forward = Vector3.forward;
    [Header("X�p�x����")]
    [SerializeField] private float _xMax, _xMin;
    [Header("Z�p�x����")]
    [SerializeField] private float _zMax, _zMin;
    [Header("�j���[�g�����͈̔�")]
    [SerializeField] private int _neutralRange;
    [SerializeField] private float _rotateSpeed = 5;

    private PlayerSetting _playerSetting;
    private Vector3 _controllerRotate = Vector3.zero;
    private Vector3 _controllerDir;
    private Vector3 _defaultRotate;
    private Vector3 _controllerSavePos;
    private bool _isLeverMove = false;

    void Start()
    {
        _defaultRotate = transform.localEulerAngles;
        //_xRGrabInteractable.firstSelectEntered.AddListener(x => _isLeverMove = true);
        //_xRGrabInteractable.lastSelectExited.AddListener(x => _isLeverMove = false);
    }

    void Update()
    {
        _isLeverMove = Convert.ToBoolean(_controllerTriggerInput.action.ReadValue<float>());
        LeverRotate2();
    }

    /// <summary>
    /// ���o�[�̃��[�e�[�V�����𐧌䂷��
    /// </summary>
    private void LeverRotate()
    {
        if (_isLeverMove)
        {
            var dir = _handTransfrom.position - transform.position;
            // �^�[�Q�b�g�̕����ւ̉�]
            //var lookAtRotation = Quaternion.LookRotation(dir, Vector3.up);
            // ��]�␳
            //var offsetRotation = Quaternion.FromToRotation(_forward, Vector3.forward);

            transform.LookAt(_handTransfrom);
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
            Vector3 dir = new();
            //���o�[����
            dir = Anglelimit(_handTransfrom.transform.localPosition - _controllerSavePos);
            
            var moveRotate = new Vector3(dir.z, 0, dir.x * -1);
            _controllerRotate += moveRotate * _rotateSpeed;
            transform.Rotate(moveRotate.x * _rotateSpeed, moveRotate.y, 0);
        }
        else 
        {
            //��𗣂����Ƃ��f�t�H�̈ʒu�ɖ߂�
            transform.Rotate(_controllerRotate.x * -1, 0, 0);
            _controllerRotate = Vector3.zero;
        }
        //Y�����͎g��Ȃ��̂ŌŒ�
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, transform.localEulerAngles.z);
        SetControllerDir();
        _controllerSavePos = _handTransfrom.transform.localPosition;
    }

    /// <summary>
    /// �p�x��������֐�
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    private Vector3 Anglelimit(Vector3 dir) 
    {
        Debug.Log(dir);
        if (_xMax < _controllerRotate.x + (dir.z * _rotateSpeed)
            || _xMin > _controllerRotate.x + (dir.z * _rotateSpeed))
        {
            dir.z = 0;
        }

        if (_zMax < _controllerRotate.z + (dir.x * -1 * _rotateSpeed)
            || _zMin > _controllerRotate.z + (dir.x * -1 * _rotateSpeed)) 
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
        if (_controllerRotate.x > 0 + _neutralRange)
        {
            _controllerDir.z = 1;
        }
        else if (_controllerRotate.x < -1 - _neutralRange)
        {
            _controllerDir.z = -1;
        }
        else 
        {
            _controllerDir.z = 0;
        }

        if (_controllerRotate.x  > 0 + _neutralRange)
        {
            _controllerDir.x = 1;
        }
        else if (_controllerRotate.x < -1 - _neutralRange)
        {
            _controllerDir.x = -1;
        }
        else 
        {
            _controllerDir.x = 0;
        }
    }
}
