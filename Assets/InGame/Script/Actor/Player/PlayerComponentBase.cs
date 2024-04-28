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

    public void SetUp(PlayerEnvroment playerEnvroment, CancellationToken token) 
    {
        _playerEnvroment = playerEnvroment;
        _playerParams = _playerEnvroment.PlayerSetting.PlayerParamsData;

    }

    protected virtual void Start()
    {
   
    }

    protected virtual void FixedUpdate() 
    {
      
    }

    protected virtual void Update() 
    {

    }

    public abstract void Dispose();
}
