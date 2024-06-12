using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class PlayerHp : MonoBehaviour, IDamageable
{
    public event Action OnDownEvent;
    [SerializeField] Camera _mainCamera;
    [SerializeField] float _time;
    [SerializeField] float _strength;  

    private int _hp;
    private PlayerEnvroment _playerEnvroment;

    public void Setup(PlayerEnvroment playerEnvroment) 
    {
        _playerEnvroment = playerEnvroment;
        _hp = _playerEnvroment.PlayerSetting.PlayerParamsData.Hp;
    }

    public void Damage(int value, string weapon = "")
    {
        _hp = Mathf.Max(_hp - value, 0);
        Debug.Log("ダメージを受けた");
        _mainCamera.DOShakePosition(_time, _strength);
        if (_hp == 0) 
        {
            OnDownEvent?.Invoke();
        }
    }
}
