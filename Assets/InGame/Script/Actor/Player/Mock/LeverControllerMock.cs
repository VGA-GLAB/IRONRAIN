using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System;

public class LeverControllerMock : MonoBehaviour
{
    /// <summary>コントローラの向いている方向</summary>
    public Vector3 ControllerDir => _controllerDir;
    [SerializeField] private InputActionProperty _controllerTriggerInput;
    [SerializeField] private Transform _handTransfrom;
    [Header("前方の基準となるローカル空間ベクトル")]
    [SerializeField] private Vector3 _forward = Vector3.forward;
    [Header("X角度制限")]
    [SerializeField] private float _xMax, _xMin;
    [Header("Z角度制限")]
    [SerializeField] private float _zMax, _zMin;
    [Header("ニュートラルの範囲")]
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
    /// レバーのローテーションを制御する
    /// </summary>
    private void LeverRotate()
    {
        if (_isLeverMove)
        {
            var dir = _handTransfrom.position - transform.position;
            // ターゲットの方向への回転
            //var lookAtRotation = Quaternion.LookRotation(dir, Vector3.up);
            // 回転補正
            //var offsetRotation = Quaternion.FromToRotation(_forward, Vector3.forward);

            transform.LookAt(_handTransfrom);
            //transform.rotation = lookAtRotation * offsetRotation;

            // Debug.Log($"xMax値:{_xMax}現在のX{transform.localEulerAngles.x}判定:{_xMax < transform.localEulerAngles.x}");

            //角度制限
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
    /// レバーのローテーションを制御する案その２
    /// </summary>
    private void LeverRotate2() 
    {
        if (_isLeverMove)
        {
            Vector3 dir = new();
            //レバー制御
            dir = Anglelimit(_handTransfrom.transform.localPosition - _controllerSavePos);
            
            var moveRotate = new Vector3(dir.z, 0, dir.x * -1);
            _controllerRotate += moveRotate * _rotateSpeed;
            transform.Rotate(moveRotate.x * _rotateSpeed, moveRotate.y, 0);
        }
        else 
        {
            //手を離したときデフォの位置に戻す
            transform.Rotate(_controllerRotate.x * -1, 0, 0);
            _controllerRotate = Vector3.zero;
        }
        //Y方向は使わないので固定
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, transform.localEulerAngles.z);
        SetControllerDir();
        _controllerSavePos = _handTransfrom.transform.localPosition;
    }

    /// <summary>
    /// 角度制限する関数
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
    /// コントローラの向いている方向をセットする
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
