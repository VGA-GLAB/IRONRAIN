using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class PlayerMoveModel : IPlayerStateModel
{
    [SerializeField] LeverController _leftController;
    [SerializeField] LeverController _rightController;
    [SerializeField] Rigidbody _rb;
    [SerializeField] private Transform _centerPoint;

    private float _totalThrusterMove;
    private PlayerEnvroment _playerEnvroment;
    private PlayerSetting.PlayerParams _params;
    private Transform _transform;
    private float _start��;

    public void SetUp(PlayerEnvroment env, CancellationToken token)
    {
        _playerEnvroment = env;
        _params = env.PlayerSetting.PlayerParamsData;
        _leftController.SetUp(env.PlayerSetting);
        _rightController.SetUp(env.PlayerSetting);  
        _transform = _playerEnvroment.PlayerTransform;
    }

    public void Start()
    {
        _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
        Sum��();
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

    public void Update()
    {
        _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
    }

    public void Dispose()
    {
       
    }

    private void Move()
    {
        //�R�M�A
        if (_leftController.ControllerDir.z == 1 && _rightController.ControllerDir.z == 1)
        {
            _rb.velocity =_transform.forward * _params.ThreeGearSpeed * ProvidePlayerInformation.TimeScale;
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
                var nextPoint = NextThrusterMovePoint(_params.ThrusterMoveNum * -1);
                Debug.Log("���X���X�^�[");
                UniTask.Create(async () =>
                {
                    await _playerEnvroment.PlayerTransform
                    .DOMove(nextPoint, _params.ThrusterMoveTime * ProvidePlayerInformation.TimeScale)
                    .OnComplete(() => _playerEnvroment.PlayerTransform.position = nextPoint);
                    _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
                    _playerEnvroment.RemoveState(PlayerStateType.Thruster);
                });
            }
        }
        //�E�X���X�^�[
        else if (_leftController.ControllerDir.z != 1 && _rightController.ControllerDir.z == 1)
        {
            //�X���X�^�[���ł͂Ȃ������ꍇ
            if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.Thruster))
            {
                _playerEnvroment.AddState(PlayerStateType.Thruster);
                var nextPoint = NextThrusterMovePoint(_params.ThrusterMoveNum);
                UniTask.Create(async () =>
                {
                    await _playerEnvroment.PlayerTransform
                    .DOMove(nextPoint, _params.ThrusterMoveTime * ProvidePlayerInformation.TimeScale)
                    .OnComplete(()=> _playerEnvroment.PlayerTransform.position = nextPoint);
                    _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
                    _playerEnvroment.RemoveState(PlayerStateType.Thruster);
                });
            }
        }
        //�Q�M�A
        else if (_leftController.ControllerDir.z == 0 && _rightController.ControllerDir.z == 0)
        {
            _rb.velocity = _transform.forward * _params.TwoGearSpeed * ProvidePlayerInformation.TimeScale;
        }
    }

    /// <summary>
    /// ���ɃX���X�^�[�ňړ�����|�C���g
    /// </summary>
    private Vector3 NextThrusterMovePoint(float moveDistance) 
    {
        var playerPos = _playerEnvroment.PlayerTransform.position;
        playerPos.y = 0;
        var centerPosition = _centerPoint.position;
        centerPosition.y = 0;
        var r = Vector3.Distance(centerPosition, playerPos);
        var �� = (moveDistance / r);
        Debug.Log($"��:{��}r:{r}");

       _totalThrusterMove += ��;
        var cos = Mathf.Cos(_totalThrusterMove + _start��);
        var x = cos * r;
        var z = Mathf.Sin(_totalThrusterMove + _start��) * r;

        var position = new Vector3 (x + _centerPoint.position.x, _playerEnvroment.PlayerTransform.position.y, z + _centerPoint.position.z);
        //Debug.Log($"�ړ����܂���X:{position.x}:Z{position.z}r:{r}");
        return position;
    }

    private void Sum��() 
    {
        var playerPos = _playerEnvroment.PlayerTransform.position;
        playerPos.y = 0;
        var centerPosition = _centerPoint.position;
        centerPosition.y = 0;

        var r = Vector3.Distance(centerPosition, playerPos);
        var aDir = (playerPos - centerPosition).normalized;
        var bDir = (new Vector3(_centerPoint.position.x + r, 0, _centerPoint.position.z) - centerPosition).normalized;
        _start�� = Vector3.Angle(aDir, bDir) * Mathf.Deg2Rad;
    }
}
