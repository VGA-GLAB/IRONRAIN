using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System;
using DG.Tweening;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;

namespace IronRain.Player
{
    public class LeverController : MonoBehaviour
    {
        /// <summary>コントローラーの向いている方向</summary>
        public Vector3 ControllerDir => _controllerDir;

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
        [SerializeField] private LeverType _leverType;
        [SerializeField] private MyButton _rightButton1;
        [SerializeField] private MyButton _rightButton2;

        private bool _isLeverMove = false;
        private Vector2 _leverDir = new();
        private Vector3 _playerHandSavePos;
        private Vector3 _controllerDir = new();
        private Vector3 _controllerMoveDir = new();
        private PlayerSetting _playerSetting;
        private Tween _tween;
        private bool _isSetUp;

        private void Start()
        {
            if (_leverType == LeverType.Right)
            {
                _rightButton1?.OnClickDown.Subscribe(_ => InputProvider.Instance.CallEnterInput(InputProvider.InputType.OneButton)).AddTo(this);
                _rightButton1?.OnClickUp.Subscribe(_ => InputProvider.Instance.CallExitInput(InputProvider.InputType.OneButton)).AddTo(this);
                _rightButton2?.OnClickDown.Subscribe(_ => InputProvider.Instance.CallEnterInput(InputProvider.InputType.TwoButton)).AddTo(this);
                _rightButton2?.OnClickUp.Subscribe(_ => InputProvider.Instance.CallExitInput(InputProvider.InputType.TwoButton)).AddTo(this);
                _isLeverMove = InputProvider.Instance.GetStayInput(InputProvider.InputType.RightTrigger);
            }
            else
            {
                _isLeverMove = InputProvider.Instance.GetStayInput(InputProvider.InputType.LeftTrigger);
            }
        }

        void Update()
        {
            if (!_isSetUp) return;

            if (!_playerSetting.IsVRInput && _leverDir != Vector2.zero)
            {
                _isLeverMove = true;
            }
            LeverMove();

            if (_leverType == LeverType.Left)
            {
                _isLeverMove = InputProvider.Instance.IsLeftLeverMove;
                InputProvider.Instance.LeftLeverDir = _controllerDir;
                _leverDir = InputProvider.Instance.LeftLeverInputDir;
            }
            else if (_leverType == LeverType.Right)
            {
                _isLeverMove = InputProvider.Instance.IsRightLeverMove;
                InputProvider.Instance.RightLeverDir = _controllerDir;
                _leverDir = InputProvider.Instance.RightLeverInputDir;
            }
        }

        public void SetUp(PlayerSetting playerSetting)
        {
            _playerSetting = playerSetting;
            _isSetUp = true;
        }

        /// <summary>
        /// レバーを制御する関数
        /// </summary>
        private void LeverMove()
        {
            if (_isLeverMove)
            {
                SetControllerDir();
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

                _controllerDir = Vector3.zero;
            }

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
            }

            _controllerMoveDir.x = 0;
            _controllerMoveDir.y = 0;
        }

        /// <summary>
        /// コントローラの方向を決める
        /// </summary>
        private void SetControllerDir()
        {
            //VRコントローラの場合
            if (_playerSetting.IsVRInput)
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
                //それ以外
                if (0.9f <= _leverDir.y)
                {
                    if (_controllerDir.z != 1)
                    {
                        CriAudioManager.Instance.SE.Play3D(transform.position, "SE", "SE_Lever");
                    }
                    _controllerDir.z = 1;
                }
                else if (_leverDir.y < -0.90f)
                {
                    if (_controllerDir.z != -1)
                    {
                        CriAudioManager.Instance.SE.Play3D(transform.position, "SE", "SE_Lever");
                    }
                    _controllerDir.z = -1;
                }
                else
                {
                    _controllerDir.z = 0;
                }

                if (0.9f <= _leverDir.x)
                {
                    if (_controllerDir.x != 1)
                    {
                        CriAudioManager.Instance.SE.Play3D(transform.position, "SE", "SE_Lever");
                    }
                    _controllerDir.x = 1;
                }
                else if (_leverDir.x < -0.9f)
                {
                    if (_controllerDir.x != -1)
                    {
                        CriAudioManager.Instance.SE.Play3D(transform.position, "SE", "SE_Lever");
                    }
                    _controllerDir.x = -1;
                }
                else
                {
                    _controllerDir.x = 0;
                }
            }
        }

        private enum LeverType
        {
            Left,
            Right,
        }
    }
}
