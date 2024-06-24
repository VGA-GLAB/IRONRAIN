using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

public abstract class PlayerComponentBase : MonoBehaviour, IDisposable
{
    protected PlayerEnvroment _playerEnvroment;
    protected PlayerSetting.PlayerParams _playerParams;
    [SerializeReference, SubclassSelector]
    protected IPlayerStateModel _playerStateModel;
    protected IPlayerStateView _playerStateView;
    protected CancellationToken _rootCancellOnDestroy;

    public void SetUp(PlayerEnvroment playerEnvroment, CancellationToken token) 
    {
        _playerEnvroment = playerEnvroment;
        _playerParams = _playerEnvroment.PlayerSetting.PlayerParamsData;
        _rootCancellOnDestroy = token;
    }

    protected virtual void Start()
    {
        _playerStateView?.SetUp(_playerEnvroment, _rootCancellOnDestroy);
        _playerStateModel?.SetUp(_playerEnvroment, _rootCancellOnDestroy);
        _playerStateModel?.Start();
    }

    protected virtual void FixedUpdate() 
    {
      _playerStateModel?.FixedUpdate();
    }

    protected virtual void Update() 
    {
        _playerStateModel?.Update();
    }

    public virtual void Dispose() 
    {
        _playerStateModel?.Dispose();
    }
}
