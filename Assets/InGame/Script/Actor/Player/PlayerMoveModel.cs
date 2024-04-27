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
    private float _start��;

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
        _dir = new Vector2(_leftController.ControllerDir.z, _rightController.ControllerDir.z);

        //�O�i
        if (_leftController.ControllerDir.z == 1 && _rightController.ControllerDir.z == 1)
        {
            _rb.velocity =_transform.forward * _threeGearSpeed * _playerEnvroment.PlayerTimeSpeed;
            //_moveState = MoveState.Forward;
        }
        //���
        else if (_leftController.ControllerDir.z == -1 && _rightController.ControllerDir.z == -1)
        {
            _rb.velocity = _transform.forward * _oneGearSpeed * _playerEnvroment.PlayerTimeSpeed;
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
                Debug.Log("���X���X�^�[");
                UniTask.Create(async () =>
                {
                    await _playerEnvroment.PlayerTransform
                    .DOMove(nextPoint, _thrusterMoveTime * _playerEnvroment.PlayerTimeSpeed)
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
                var nextPoint = NextThrusterMovePoint(_thrusterMoveNum);
                UniTask.Create(async () =>
                {
                    await _playerEnvroment.PlayerTransform
                    .DOMove(nextPoint, _thrusterMoveTime * _playerEnvroment.PlayerTimeSpeed)
                    .OnComplete(()=> _playerEnvroment.PlayerTransform.position = nextPoint);
                    _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
                    _playerEnvroment.RemoveState(PlayerStateType.Thruster);
                });
            }
        }
        else if (_leftController.ControllerDir.z == 0 && _rightController.ControllerDir.z == 0)
        {
            Debug.Log(_playerEnvroment.PlayerTimeSpeed);
            _rb.velocity = _transform.forward * _twoGearSpeed * _playerEnvroment.PlayerTimeSpeed;
        }
    }

    /// <summary>
    /// ���ɃX���X�^�[�ňړ�����|�C���g
    /// </summary>
    private Vector3 NextThrusterMovePoint(float moveDistance) 
    {
        var playerPos = _playerEnvroment.PlayerTransform.position;
        playerPos.y = 0;
        var r = Vector3.Distance(_centerPoint.position, playerPos);
        var �� = (moveDistance / r);
        Debug.Log($"��:{��}r:{r}");

       _totalThrusterMove += ��;
        var cos = Mathf.Cos(_totalThrusterMove + _start��);
        var x = cos * r;
        var z = Mathf.Sin(_totalThrusterMove + _start��) * r;

        var position = new Vector3 (x + _centerPoint.position.x, _playerEnvroment.PlayerTransform.position.y, z + _centerPoint.position.z);
        Debug.Log($"�ړ����܂���X:{position.x}:Z{position.z}r:{r}");
        return position;
    }

    private void Sum��() 
    {
        var r = Vector3.Distance(_centerPoint.position, _playerEnvroment.PlayerTransform.position);
        var aDir = (_playerEnvroment.PlayerTransform.position - _centerPoint.position).normalized;
        var bDir = (new Vector3(_centerPoint.position.x + r, _centerPoint.position.y, _centerPoint.position.z) - _centerPoint.position).normalized;
        _start�� = Vector3.Angle(aDir, bDir) * Mathf.Deg2Rad;
    }
}
