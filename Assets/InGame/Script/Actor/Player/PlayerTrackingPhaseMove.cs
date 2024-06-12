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

    [Tooltip("現在のレーン")]
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
    protected override void FixedUpdate()
    {        
        base.FixedUpdate();
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

    protected override void Update()
    {
        base.Update();
    }

    public override void Dispose()
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
                var nextPoint = transform.position + transform.right * -1 * _params.ThrusterMoveNum;
                ThrusterMove(nextPoint);
                _currentLane--;
                if (_currentLane == _params.RestrictionLane * -1) _savePos = transform.localPosition;
            }
            _rb.velocity = _transform.forward * _params.Speed * ProvidePlayerInformation.TimeScale;
        }
        //右スラスター
        else if (_rightController.ControllerDir.x == 1)
        {
            //スラスター中ではなかった場合
            if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.Thruster))
            {
                _playerEnvroment.AddState(PlayerStateType.Thruster);
                var nextPoint = transform.position + transform.right * _params.ThrusterMoveNum;
                ThrusterMove(nextPoint);
                _currentLane++;
                if (_currentLane == _params.RestrictionLane) _savePos = transform.localPosition;
            }
            _rb.velocity = _transform.forward * _params.Speed * ProvidePlayerInformation.TimeScale;
        }
        //２ギア
        else
        {
            _rb.velocity = _transform.forward * _params.Speed * ProvidePlayerInformation.TimeScale;
        }

        _rb.velocity += transform.right * ReturnLaneStrength();
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
            //Debug.Log("0");
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

        var dis = _transform.localPosition.x - _savePos.x;
        //Debug.Log($"距離{dis}");
        if (Mathf.Abs(dis) < 2 && _isRetunRale)
        {
            //Debug.Log("付いた");
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
