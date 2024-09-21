using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LeverAnimation : MonoBehaviour
{
    [SerializeField] private InputProvider.InputType _inputType;
    [Header("前後できるレバーの位置")]
    [SerializeField] private Transform _frontPos;
    [SerializeField] private Transform _backPos;
    [SerializeField] private Transform _defaultPos;
    [SerializeField] private Transform _lookAt;
    [SerializeField] private Animator _amim;
    [Header("FryLeverの位置")]
    [SerializeField] private float _rightAngle;
    [SerializeField] private float _leftAngle;
    [SerializeField] private float _frontAngle;
    [SerializeField] private float _backAngle;

    [Header("移動にかかる時間")]
    [SerializeField] private float _duration = 0.5f;

    private Vector2 _saveVector2Input;
    private Vector3 _saveVector3Input;
    private Tween _tween;


    private void Update()
    {
        if (_lookAt != null) 
        {
            transform.LookAt(_lookAt);
        }
        
        if (_inputType == InputProvider.InputType.FryLever)
        {
            FryLeverAnim();
        }
        else
        {
            LeverAnim();
        }
    }

    private void LeverAnim()
    {
        if (_inputType == InputProvider.InputType.ThrottleLever)
        {
            if (_saveVector3Input == InputProvider.Instance.LeftLeverDir) return;
            _saveVector3Input = InputProvider.Instance.LeftLeverDir;

            if (_tween != null)
            {
                _tween.Complete();
            }

            if (InputProvider.Instance.LeftLeverDir.y == 1)
            {
                _tween = transform.DOMove(_frontPos.position, _duration)
                    .OnComplete(() => transform.position = _frontPos.position)
                    .SetLink(gameObject);
            }
            else if (InputProvider.Instance.LeftLeverDir.y == -1)
            {
                _tween = transform.DOMove(_backPos.position, _duration)
                    .OnComplete(() => transform.position = _backPos.position)
                    .SetLink(gameObject);
            }
            else
            {
                _tween = transform.DOMove(_defaultPos.position, _duration)
                    .OnComplete(() => transform.position = _defaultPos.position)
                    .SetLink(gameObject);
            }
        }
        else if (_inputType == InputProvider.InputType.ThreeLever)
        {
            if (_saveVector2Input == InputProvider.Instance.ThreeLeverDir) return;
            _saveVector2Input = InputProvider.Instance.ThreeLeverDir;

            if (_tween != null)
            {
                _tween.Complete();
            }

            if (InputProvider.Instance.ThreeLeverDir.y == 1)
            {
                _tween = transform.DOMove(_frontPos.position, _duration)
                    .OnComplete(() => transform.position = _frontPos.position)
                    .SetLink(gameObject);
            }
            else if (InputProvider.Instance.ThreeLeverDir.y == -1)
            {
                _tween = transform.DOMove(_backPos.position, _duration)
                    .OnComplete(() => transform.position = _backPos.position)
                    .SetLink(gameObject);
            }
            else
            {
                _tween = transform.DOMove(_defaultPos.position, _duration)
                    .OnComplete(() => transform.position = _defaultPos.position)
                    .SetLink(gameObject);
            }
        }
        else if (_inputType == InputProvider.InputType.FourLever) 
        {
            if (_saveVector2Input == InputProvider.Instance.FourLeverDir) return;
            _saveVector2Input = InputProvider.Instance.FourLeverDir;

            if (_tween != null)
            {
                _tween.Complete();
            }

            if (InputProvider.Instance.FourLeverDir.y == 1)
            {
                _tween = transform.DOMove(_frontPos.position, _duration)
                    .OnComplete(() => transform.position = _frontPos.position)
                    .SetLink(gameObject);
            }
            else if (InputProvider.Instance.FourLeverDir.y == -1)
            {
                _tween = transform.DOMove(_backPos.position, _duration)
                    .OnComplete(() => transform.position = _backPos.position)
                    .SetLink(gameObject);
            }
            else
            {
                _tween = transform.DOMove(_defaultPos.position, _duration)
                    .OnComplete(() => transform.position = _defaultPos.position)
                    .SetLink(gameObject);
            }
        }
    }

    private void FryLeverAnim()
    {

        if (_saveVector3Input == InputProvider.Instance.RightLeverDir) return;
        _saveVector3Input = InputProvider.Instance.RightLeverDir;


        _amim.SetFloat("X", InputProvider.Instance.RightLeverDir.x);


        if (_tween != null)
        {
            _tween.Complete();
        }

        if (InputProvider.Instance.RightLeverDir.x == 1)
        {

            //_tween = DOTween.To(() => transform.rotation.z,
            //     x => transform.rotation = Quaternion.Euler(0, 0, x),
            //     _rightAngle, _duration)
            //     .OnComplete(() => transform.rotation = Quaternion.Euler(0, 0, _rightAngle));

            //var value = _rightAngle - transform.eulerAngles.z;

            //transform.DORotate(new Vector3(0,0,30), _duration, RotateMode.FastBeyond360);

            //_tween = transform.DORotate(new Vector3(0, 0, _rightAngle), _duration)
            //    .OnComplete(() => transform.Rotate(0, 0, _rightAngle))
            //    .SetLink(gameObject);
        }
        else if (InputProvider.Instance.RightLeverDir.x == -1)
        {

            //var value = _leftAngle - transform.eulerAngles.z;
            //transform.DORotate(new Vector3(0, 0, -30), _duration, RotateMode.FastBeyond360);

            //_tween = DOTween.To(() => transform.rotation.z,
            //    x => transform.rotation = Quaternion.Euler(0,0,x),
            //     _leftAngle, _duration)
            //     .OnComplete(() => transform.rotation = Quaternion.Euler(0, 0, _leftAngle));

            //_tween = transform.DORotate(new Vector3(0, 0, _leftAngle), _duration)
            //    .OnComplete(() => transform.Rotate(0, 0, _leftAngle))
            //    .SetLink(gameObject);
        }
        else if (InputProvider.Instance.RightLeverDir.y == 1)
        {
            _tween = transform.DORotate(new Vector3(_frontAngle, 0, 0), _duration)
                .OnComplete(() => transform.Rotate(_frontAngle, 0, 0))
                .SetLink(gameObject);
        }
        else if (InputProvider.Instance.RightLeverDir.y == -1)
        {
            _tween = transform.DORotate(new Vector3(_backAngle, 0, 0), _duration)
                .OnComplete(() => transform.Rotate(_backAngle, 0, 0))
                .SetLink(gameObject);
        }
        else
        {

            //_tween = DOTween.To(() => transform.eulerAngles.z,
            //    x => transform.eulerAngles = new Vector3(0, 0, 0),
            //    0, _duration)
            //    .OnComplete(() => transform.eulerAngles = new Vector3(0, 0, 0));

            //var value = 0 - transform.eulerAngles.z;
            //transform.DORotate(new Vector3(0, 0, 0), _duration, RotateMode.FastBeyond360);

            //_tween = transform.DORotate(new Vector3(0, 0, 0), _duration)
            //    .OnComplete(() => transform.Rotate(0, 0, 0))
            //    .SetLink(gameObject);
        }
    }

    private enum LeverType 
    {

    }
}
