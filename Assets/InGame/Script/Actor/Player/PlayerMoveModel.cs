using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enemy.Control.Boss;

public class PlayerMoveModel : IPlayerStateModel
{
    [SerializeField] LeverController _leftController;
    [SerializeField] LeverController _rightController;
    [SerializeField] Rigidbody _rb;
    [SerializeField] private BossStage _bossStage;

    private float _totalThrusterMove;
    private PlayerEnvroment _playerEnvroment;
    private PlayerSetting.PlayerParams _params;
    private Transform _transform;
    private float _startTheta;
    private Transform _centerPoint;

    public void SetUp(PlayerEnvroment env, CancellationToken token)
    {
        _playerEnvroment = env;
        _params = env.PlayerSetting.PlayerParamsData;
        _leftController.SetUp(env.PlayerSetting);
        _rightController.SetUp(env.PlayerSetting);  
        _transform = _playerEnvroment.PlayerTransform;
        _centerPoint = _bossStage.PointP;
        env.PlayerTransform.SetParent(_centerPoint);
        _playerEnvroment.PlayerTransform.position = new Vector3(0, 0, 20);
    }

    public void Start()
    {
        _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
        SumTheta();
    }
    public void FixedUpdate()
    {
        if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE) || !_playerEnvroment.PlayerState.HasFlag(PlayerStateType.Inoperable))
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
        //左スラスター
        if (_rightController.ControllerDir.x == -1)
        {
            //スラスター中ではなかった場合
            if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.Thruster))
            {
                _playerEnvroment.AddState(PlayerStateType.Thruster);
                var nextPoint = NextThrusterMovePoint(_params.ThrusterMoveNum * -1);
                Debug.Log("左スラスター");
                UniTask.Create(async () =>
                {
                    await _playerEnvroment.PlayerTransform
                    .DOLocalMove(nextPoint, _params.ThrusterMoveTime * ProvidePlayerInformation.TimeScale)
                    .OnComplete(() => _playerEnvroment.PlayerTransform.localPosition = nextPoint);
                    _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
                    _playerEnvroment.RemoveState(PlayerStateType.Thruster);
                });
            }
        }
        //右スラスター
        else if (_rightController.ControllerDir.x == 1)
        {
            //スラスター中ではなかった場合
            if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.Thruster))
            {
                _playerEnvroment.AddState(PlayerStateType.Thruster);
                var nextPoint = NextThrusterMovePoint(_params.ThrusterMoveNum);
                UniTask.Create(async () =>
                {
                    await _playerEnvroment.PlayerTransform
                    .DOLocalMove(nextPoint, _params.ThrusterMoveTime * ProvidePlayerInformation.TimeScale)
                    .OnComplete(()=> _playerEnvroment.PlayerTransform.localPosition = nextPoint);
                    _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
                    _playerEnvroment.RemoveState(PlayerStateType.Thruster);
                });
            }
        }
        //２ギア
        else
        {
            _rb.velocity = _transform.forward * _params.Speed * ProvidePlayerInformation.TimeScale;
        }
    }

    /// <summary>
    /// 次にスラスターで移動するポイント
    /// </summary>
    private Vector3 NextThrusterMovePoint(float moveDistance) 
    {
        var playerPos = _playerEnvroment.PlayerTransform.localPosition;
        playerPos.y = 0;
        var centerPosition = Vector3.zero;
        centerPosition.y = 0;
        var r = Vector3.Distance(centerPosition, playerPos);
        var theta = (moveDistance / r);
        Debug.Log($"θ:{theta}r:{r}");

       _totalThrusterMove += theta;
        var cos = Mathf.Cos(_totalThrusterMove + _startTheta);
        var x = cos * r;
        var z = Mathf.Sin(_totalThrusterMove + _startTheta) * r;

        var position = new Vector3 (x + centerPosition.x, _playerEnvroment.PlayerTransform.localPosition.y, z + centerPosition.z);
        //Debug.Log($"移動しましたX:{position.x}:Z{position.z}r:{r}");
        return position;
    }

    private void SumTheta() 
    {
        var playerPos = _playerEnvroment.PlayerTransform.localPosition;
        playerPos.y = 0;
        var centerPosition = Vector3.zero;
        centerPosition.y = 0;

        var r = Vector3.Distance(centerPosition, playerPos);
        var aDir = (playerPos - centerPosition).normalized;
        var bDir = (new Vector3(r, 0, centerPosition.z) - centerPosition).normalized;
        _startTheta = Vector3.Angle(aDir, bDir) * Mathf.Deg2Rad;
    }
}
