using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerTrackingPhaseMove : PlayerComponentBase
{
    [SerializeField] LeverController _leftController;
    [SerializeField] LeverController _rightController;
    [SerializeField] Rigidbody _rb;
    [SerializeField] private Transform _centerPoint;

    private PlayerSetting.PlayerParams _params;
    private Transform _transform;

    [Tooltip("���݂̃��[��")]
    private int _currentLane;
    private Vector3 _savePos;
    private bool _isRetunRale;

    protected override void Start()
    {
        base.Start();
        _params = _playerEnvroment.PlayerSetting.PlayerParamsData;
        _leftController.SetUp(_playerEnvroment.PlayerSetting);
        _rightController.SetUp(_playerEnvroment.PlayerSetting);
        _transform = _playerEnvroment.PlayerTransform;

        _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
    }
    public void FixedUpdate()
    {
        if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE))
        {
            Move();
        }
        else
        {
            _rb.velocity = Vector2.zero;
        }
    }

    public void Dispose()
    {

    }

    private void Move()
    {
        //�R�M�A
        if (_leftController.ControllerDir.z == 1 && _rightController.ControllerDir.z == 1)
        {
            _rb.velocity = _transform.forward * _params.ThreeGearSpeed * ProvidePlayerInformation.TimeScale;
        }
        //�P�M�A
        else if (_leftController.ControllerDir.z == -1 && _rightController.ControllerDir.z == -1)
        {
            _rb.velocity = _transform.forward * _params.OneGearSpeed * ProvidePlayerInformation.TimeScale;
        }
        //���X���X�^�[
        else if (_leftController.ControllerDir.z == 1 && _rightController.ControllerDir.z != 1)
        {
            //�X���X�^�[���ł͂Ȃ������ꍇ
            if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.Thruster))
            {
                _playerEnvroment.AddState(PlayerStateType.Thruster);
                var nextPoint = transform.position + transform.right * -1 * _params.ThrusterMoveNum;
                ThrusterMove(nextPoint);
                _currentLane--;
                if (_currentLane == _params.RestrictionLane * -1) _savePos = transform.localPosition;
            }
            _rb.velocity = _transform.forward * _params.TwoGearSpeed * ProvidePlayerInformation.TimeScale;
        }
        //�E�X���X�^�[
        else if (_leftController.ControllerDir.z != 1 && _rightController.ControllerDir.z == 1)
        {
            //�X���X�^�[���ł͂Ȃ������ꍇ
            if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.Thruster))
            {
                _playerEnvroment.AddState(PlayerStateType.Thruster);
                var nextPoint = transform.position + transform.right * _params.ThrusterMoveNum;
                ThrusterMove(nextPoint);
                _currentLane++;
                if (_currentLane == _params.RestrictionLane) _savePos = transform.localPosition;
            }
            _rb.velocity = _transform.forward * _params.TwoGearSpeed * ProvidePlayerInformation.TimeScale;
        }
        //�Q�M�A
        else if (_leftController.ControllerDir.z == 0 && _rightController.ControllerDir.z == 0)
        {
            _rb.velocity = _transform.forward * _params.TwoGearSpeed * ProvidePlayerInformation.TimeScale;
        }

        _rb.velocity += transform.right * ReturnLaneStrength();
    }

    /// <summary>
    /// �ǂ̂��炢�̑����ň�Ԓ[�̃��[���ɖ߂����ǂ���
    /// </summary>
    public float ReturnLaneStrength()
    {
        var ret = 0f;
        var isRight = 1;
        if (_currentLane <= _params.RestrictionLane && _currentLane >= _params.RestrictionLane * -1)
        {
            //Debug.Log("0");
            return 0;
        }
        if (_currentLane < _params.RestrictionLane * -1)
        {
            isRight = -1;
        }

        //�ǂ̒��x���[�����痣�ꂽ��
        var distanceLane = _currentLane - _params.RestrictionLane * isRight;
        ret = distanceLane * _params.ReturnLaneStrength;

        //�ő�l�ȏ�ɂȂ��Ă��Ȃ���
        if (ret < _params.MaxReturnLaneStrength * isRight)
        {
            ret = _params.MaxReturnLaneStrength * isRight;
        }

        var dis = _transform.localPosition.x - _savePos.x;
        //Debug.Log($"����{dis}");
        if (Mathf.Abs(dis) < 2 && _isRetunRale)
        {
            //Debug.Log("�t����");
            _isRetunRale = false;
            _currentLane = _params.RestrictionLane * isRight;
        }

        return ret * -1;
    }

    private void ThrusterMove(Vector3 nextPoint)
    {
        UniTask.Create(async () =>
        {
            await _playerEnvroment.PlayerTransform
            .DOMove(nextPoint, _params.ThrusterMoveTime * ProvidePlayerInformation.TimeScale);
            _playerEnvroment.RemoveState(PlayerStateType.Thruster);

            if (_currentLane > _params.RestrictionLane || _currentLane < _params.RestrictionLane * -1 && !_isRetunRale)
            {
                _isRetunRale = true;
            }
        });
    }
}
