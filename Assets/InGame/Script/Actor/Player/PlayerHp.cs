using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class PlayerHp : MonoBehaviour, IDamageable
{
    public event Action OnDownEvent;
    [SerializeField] Camera _mainCamera;
    [SerializeField] private float _time;
    [SerializeField] private float _strength;
    [SerializeField] private HpManager _hpManager;

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
        _hpManager.BodyDamage(value);
        if (_hp == 0) 
        {
            OnDownEvent?.Invoke();
        }
    }
}
