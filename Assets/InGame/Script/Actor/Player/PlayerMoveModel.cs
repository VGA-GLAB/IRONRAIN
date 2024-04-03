using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class PlayerMoveModel : IPlayerStateModel
{
    public Vector2 Dir => _dir;

    [SerializeField] LeverController _leftController;
    [SerializeField] LeverController _rightController;
    [SerializeField] Rigidbody _rb;
    [SerializeField] private float _oneGearSpeed;
    [SerializeField] private float _twoGearSpeed;
    [SerializeField] private float _threeGearSpeed;
    [Header("�P��̃X���X�^�[�̈ړ���")]
    [SerializeField] private float _thrusterMoveNum;
    [Header("���b�ԂŃX���X�^�[�ňړ����邩")]
    [SerializeField] private int _thrusterMoveTime;
    [SerializeField] private Transform _centerPoint;
    [SerializeField] private Transform _insPos;

    private float _totalThrusterMove;
    private PlayerEnvroment _playerEnvroment;
    private Vector2 _dir = new();
    private Transform _transform;


    public void SetUp(PlayerEnvroment env, CancellationToken token)
    {
        _playerEnvroment = env;
        _leftController.SetUp(env.PlayerSetting);
        _rightController.SetUp(env.PlayerSetting);  
        _transform = _playerEnvroment.PlayerTransform;
    }

    public void Start()
    {
        _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
    }
    public void FixedUpdate()
    {
        Move();
    }

    public void Update()
    {
        Debug.Log(_playerEnvroment.PlayerState.HasFlag(PlayerStateType.Thruster));
        _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
    }

    public void Dispose()
    {
       
    }

    private void Move()
    {
        _dir = new Vector2(_leftController.ControllerDir.z, _rightController.ControllerDir.z);

        //�O�i
        if (_leftController.ControllerDir.z == 1)
        {
            _rb.velocity = _transform.forward * _threeGearSpeed;
            //_moveState = MoveState.Forward;
        }
        //���
        else if (_leftController.ControllerDir.z == -1)
        {
            _rb.velocity = _transform.forward * _oneGearSpeed;
            //_moveState = MoveState.Back;
        }
        //���X���X�^�[
        else if (_leftController.ControllerDir.z == 1 && _rightController.ControllerDir.z != 1)
        {
            //�X���X�^�[���ł͂Ȃ������ꍇ
            if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.Thruster))
            {
                _playerEnvroment.AddState(PlayerStateType.Thruster);
                var nextPoint = NextThrusterMovePoint(_thrusterMoveNum * -1);
                UniTask.Create(async () =>
                {
                    await _playerEnvroment.PlayerTransform.DOMove(nextPoint, _thrusterMoveTime);
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
                var nextPoint = NextThrusterMovePoint(_thrusterMoveNum);
                UniTask.Create(async () =>
                {
                    await _playerEnvroment.PlayerTransform.DOMove(nextPoint, _thrusterMoveTime);
                    _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
                    _playerEnvroment.RemoveState(PlayerStateType.Thruster);
                });
            }
        }
        else if (_leftController.ControllerDir.z == 0 && _rightController.ControllerDir.z == 0)
        {
            _rb.velocity = _transform.forward * _twoGearSpeed;
        }
    }

    /// <summary>
    /// ���ɃX���X�^�[�ňړ�����|�C���g
    /// </summary>
    private Vector3 NextThrusterMovePoint(float moveDistance) 
    {
        _totalThrusterMove += moveDistance;
        var distance = Vector3.Distance(_centerPoint.position, _playerEnvroment.PlayerTransform.position);
        var �� = (_totalThrusterMove / distance) * Mathf.Deg2Rad;

        var x = Mathf.Cos(��) * distance;
        var z = Mathf.Sin(��) * distance;

        var position = new Vector3 (x + _centerPoint.position.x, _playerEnvroment.PlayerTransform.position.y, z + _centerPoint.position.z);
        return position;
    }
}
