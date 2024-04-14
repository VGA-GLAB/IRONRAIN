using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : PlayerStateBase
{
    [SerializeField] private InputActionProperty _weaponChangeInput;
    [SerializeField] private InputActionProperty _shotInput;
    [SerializeField] private List<PlayerWeapon> _playerWeaponList = new();

    private PlayerWeapon _currentPlayerWeapon;
    private bool _isWeaponChenged;
    private int _currentWeaponIndex;

    protected override void Start()
    {
        _isWeaponChenged = Convert.ToBoolean(_weaponChangeInput.action.ReadValue<float>());
    }

    protected override void Update()
    {
        if (_isWeaponChenged) 
        {

        }
    }

    /// <summary>
    /// ïêäÌêÿÇËë÷Ç¶
    /// </summary>
    private void WeaponChenge() 
    {
        
    }

    public override void Dispose()
    {

    }
}
