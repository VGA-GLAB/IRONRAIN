using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System;
using DG.Tweening;

public class LeverController : MonoBehaviour
{
    /// <summary>コントローラーの向いている方向</summary>
    public Vector3 ControllerDir => _controllerDir;

    [SerializeField] private InputActionProperty _playerHandTriggerInput;
    [SerializeField] private InputActionProperty _leverInput;
    [SerializeField] private Transform _playerHandTransfrom;
    [Header("レバーの感度設定")]
    [SerializeField] private float _moveSpeed = 5;
    [Header("X移動制限")]
    [SerializeField] private float _xMax, _xMin;
    [Header("Z移動制限")]
    [SerializeField] private float _zMax, _zMin;
    [Header("ニュートラルの位置")]
    [SerializeField] private Transform _neutralPos;
    [Header("ニュートラルの範囲")]
    [SerializeField] private float _neutralRange;

    private bool _isLeverMove = false;
    private Vector2 _leverDir;
    private Vector3 _playerHandSavePos;
    private Vector3 _controllerDir = new();
    private Vector3 _controllerMoveDir = new();
    private PlayerSetting _playerSetting;
    private Tween _tween;

    private void Start()
    {
       
    }

    void Update()
    {
        _isLeverMove = Convert.ToBoolean(_playerHandTriggerInput.action.ReadValue<float>());
        _leverDir = _leverInput.action.ReadValue<Vector2>();
        if (_playerSetting.IsKeyBoard && _leverDir != Vector2.zero) 
        {
            _isLeverMove = true;
        }
        LeverMove();
    }

    public void SetUp(PlayerSetting playerSetting) 
    {
        _playerSetting = playerSetting;
    }

    /// <summary>
    /// レバーを制御する関数
    /// </summary>
    private void LeverMove()
    {
        if (_isLeverMove)
        {
            SetControllerDir();
            MoveLimit(_playerHandTransfrom.transform.localPosition - _playerHandSavePos);
            var moveValue = _controllerMoveDir * _moveSpeed;
            transform.localPosition += moveValue;
        }
        else 
        {
            if (_tween == null & transform.localPosition != _neutralPos.localPosition) 
            {
                //ニュートラルに戻す
                _tween = transform
                    .DOLocalMoveZ(_neutralPos.localPosition.z, 0.2f)
                    .OnComplete(() => _tween = null)
                    .SetLink(this.gameObject);
            }

            _controllerDir.z = 0;

        }

        _playerHandSavePos = _playerHandTransfrom.transform.localPosition;
    }

    /// <summary>
    /// 移動の制限をつける関数
    /// </summary>
    private void MoveLimit(Vector3 dir) 
    {
        _controllerMoveDir = dir;
        //初期位置からどのくらいレバーが動いたら
        var xPos = transform.localPosition.z - _neutralPos.transform.localPosition.z;
        if (_xMax < xPos + (dir.z * _moveSpeed)
        || _xMin > xPos + (dir.z * _moveSpeed)) 
        {
            _controllerMoveDir.z = 0;
            Debug.Log("移動ダメ");
        }

        _controllerMoveDir.x = 0;
        _controllerMoveDir.y = 0;
    }

    /// <summary>
    /// コントローラの方向を決める
    /// </summary>
    private void SetControllerDir() 
    {

        if (!_playerSetting.IsKeyBoard)
        {
            if (transform.localPosition.z > _neutralPos.localPosition.z + _neutralRange)
            {
                _controllerDir.z = 1;
            }
            else if (transform.localPosition.z < _neutralPos.localPosition.z - _neutralRange)
            {
                _controllerDir.z = -1;
            }
            else
            {
                _controllerDir.z = 0;
            }
        }
        else
        {
            if (1 <= _leverDir.y)
            {
                _controllerDir.z = 1;
            }
            else if (_leverDir.y < 0)
            {
                _controllerDir.z = -1;
            }
            else 
            {
                _controllerDir.z = 0;
            }
        }
    }
}
