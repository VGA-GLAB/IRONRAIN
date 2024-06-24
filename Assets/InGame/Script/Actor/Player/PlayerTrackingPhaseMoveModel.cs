using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerTrackingPhaseMoveModel : IPlayerStateModel
{
    [SerializeField] LeverController _leftController;
    [SerializeField] LeverController _rightController;
    [SerializeField] Rigidbody _rb;
    [SerializeField] private Transform _centerPoint;

    private PlayerSetting.PlayerParams _params;
    private Transform _transform;

    [Tooltip("現在のレーン")]
    private int _currentLane;
    private Vector3 _savePos;
    [Tooltip("レーンに戻され始めているかどうか")]
    private bool _isRetunRale;
    [Tooltip("レーンに強制移動を始めるかどうか")]
    private bool _isForcingMove;
    private PlayerEnvroment _playerEnvroment;
    private CancellationToken _rootDestroyToken;


    public void SetUp(PlayerEnvroment env, CancellationToken token)
    {
        _playerEnvroment = env;
        _rootDestroyToken = token;
    }

    public void Start()
    {
        _params = _playerEnvroment.PlayerSetting.PlayerParamsData;
        _leftController.SetUp(_playerEnvroment.PlayerSetting);
        _rightController.SetUp(_playerEnvroment.PlayerSetting);
        _transform = _playerEnvroment.PlayerTransform;

        _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
    }
    public void FixedUpdate()
    {
        if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE)
            && !_playerEnvroment.PlayerState.HasFlag(PlayerStateType.Inoperable))
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

    }

    public void Dispose()
    {

    }

    private void Move()
    {
        ThrusterMove();

        if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.NonMoveForward))
        {
            _rb.velocity = _transform.forward * _params.Speed * ProvidePlayerInformation.TimeScale;
        }
        _rb.velocity += _transform.right * ReturnLaneStrength();
    }

    /// <summary>
    /// スラスターの移動処理
    /// </summary>
    private void ThrusterMove()
    {
        if (_playerEnvroment.PlayerState.HasFlag(PlayerStateType.Thruster)
            || _isForcingMove)
            return;

        //左スラスター
        if (_rightController.ControllerDir.x == -1)
        {
            _playerEnvroment.AddState(PlayerStateType.Thruster);
            var nextPoint = _transform.position + _transform.right * -1 * _params.ThrusterMoveNum;
            ThrusterNextPointMove(nextPoint).Forget();
            _currentLane--;
            if (_currentLane == _params.RestrictionLane * -1)
            {
                _savePos = _transform.position;
            }
            CriAudioManager.Instance.SE.Play("SE", "SE_Evasion");
        }
        //右スラスター
        else if (_rightController.ControllerDir.x == 1)
        {
            _playerEnvroment.AddState(PlayerStateType.Thruster);
            var nextPoint = _transform.position + _transform.right * _params.ThrusterMoveNum;
            ThrusterNextPointMove(nextPoint).Forget();
            _currentLane++;
            if (_currentLane == _params.RestrictionLane) _savePos = _transform.position;
            CriAudioManager.Instance.SE.Play("SE", "SE_Evasion");
        }
    }

    /// <summary>
    /// どのくらいの速さで一番端のレーンに戻すかどうか
    /// </summary>
    public float ReturnLaneStrength()
    {
        var ret = 0f;
        var isRight = 1;
        if (_currentLane <= _params.RestrictionLane && _currentLane >= _params.RestrictionLane * -1)
        {
            return 0;
        }
        if (_currentLane < _params.RestrictionLane * -1)
        {
            isRight = -1;
        }

        //どの程度レーンから離れたか
        var distanceLane = _currentLane - _params.RestrictionLane * isRight;
        ret = distanceLane * _params.ReturnLaneStrength;

        //最大値以上になっていないか
        if (ret < _params.MaxReturnLaneStrength * isRight)
        {
            ret = _params.MaxReturnLaneStrength * isRight;
        }

        var dis = _transform.position.z - _savePos.z;
        var endDis = 2;
        //Debug.Log($"距離{dis}");

        //ある程度近づいたら強制移動をやめる
        if (Mathf.Abs(dis) < endDis && _isRetunRale)
        {
            //Debug.Log("付いた");
            _isRetunRale = false;
            _isForcingMove = false;
            _currentLane = _params.RestrictionLane * isRight;
        }

        return ret * -1;
    }

    /// <summary>
    /// スラスターで指定されたポイントに移動する際の処理
    /// </summary>
    /// <param name="nextPoint"></param>
    private async UniTask ThrusterNextPointMove(Vector3 nextPoint)
    {
        await _playerEnvroment.PlayerTransform
        .DOMove(nextPoint, _params.ThrusterMoveTime * ProvidePlayerInformation.TimeScale)
        .ToUniTask(cancellationToken: _rootDestroyToken);
        _playerEnvroment.RemoveState(PlayerStateType.Thruster);

        //Playerを指定のレーンに戻しを始めるかどうか
        if (_currentLane > _params.RestrictionLane || _currentLane < _params.RestrictionLane * -1 && !_isRetunRale)
        {
            _isRetunRale = true;
        }

        //Playerを指定のレーンまで強制移動するかどうか
        if (_currentLane > _params.ForcingMoveLane || _currentLane < _params.ForcingMoveLane * -1 && !_isForcingMove)
        {
            _isForcingMove = true;
        }
    }
}
