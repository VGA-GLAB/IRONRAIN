using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : PlayerComponentBase
{
    public PlayerWeaponModel WeaponModel { get; private set; }

    private void Awake()
    {
        WeaponModel = _playerStateModel as PlayerWeaponModel;
    }
}
