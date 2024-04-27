using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

public abstract class PlayerComponentBase : MonoBehaviour, IDisposable
{
    public IPlayerStateModel PlayerStateModel => _playerStateModel;
    public IPlayerStateView PlayerStateView => _playerStateView;

    [SubclassSelector, SerializeReference]
    protected IPlayerStateModel _playerStateModel;
    protected IPlayerStateView _playerStateView;
    protected PlayerEnvroment _playerEnvroment;

    public void SetUp(PlayerEnvroment playerEnvroment, CancellationToken token) 
    {
        _playerEnvroment = playerEnvroment;
        _playerStateModel?.SetUp(playerEnvroment, token);
        _playerStateView?.SetUp(playerEnvroment, token);
    }

    protected virtual void Start()
    {
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

    public abstract void Dispose();
}
