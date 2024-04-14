using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHp : MonoBehaviour, IDamageable
{
    public event Action OnDownEvent;
    [SerializeField] private int _hp;

    private PlayerEnvroment _playerEnvroment;

    public void Setup(PlayerEnvroment playerEnvroment) 
    {
        _playerEnvroment = playerEnvroment;
    }

    public void Damage(int value, string weapon = "")
    {
        _hp = Mathf.Max(_hp - value, 0);

        if (_hp == 0) 
        {
            OnDownEvent?.Invoke();
        }
    }
}
