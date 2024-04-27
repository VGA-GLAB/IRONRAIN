using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : PlayerComponentBase
{
    public PlayerWeaponBase CurrentWeapon => _playerWeaponList[_currentWeaponIndex];

    [SerializeField] private InputActionProperty _shotInput;
    [SerializeField] private List<PlayerWeaponBase> _playerWeaponList = new();

    private bool _isWeaponChenge;
    private bool _isShot;
    //0n‚Ü‚è
    private int _currentWeaponIndex;

    protected override void Start()
    {
        base.Start();
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.WeaponChenge, WeaponChenge);
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.Shot, Shot);

        for (int i = 0; i < _playerWeaponList.Count; i++) 
        {
            _playerWeaponList[i].SetUp(_playerEnvroment);
        }
    }


    protected override void Update()
    {
        if (_playerEnvroment.PlayerState.HasFlag(PlayerStateType.SwitchingArms)
            && _playerEnvroment.PlayerState.HasFlag(PlayerStateType.RepairMode)) return;
    }

    /// <summary>
    /// •ŠíØ‚è‘Ö‚¦
    /// </summary>
    private void WeaponChenge() 
    {
        if (_currentWeaponIndex + 1 < _playerWeaponList.Count)
        {
            _currentWeaponIndex++;
        }
        else 
        {
            _currentWeaponIndex = 0;
        }
        Debug.Log($"Œ»İ‚Ì•Ší‚Í{_playerWeaponList[_currentWeaponIndex]}");
    }

    private void Shot() 
    {
        _playerWeaponList[_currentWeaponIndex].Shot();
    }

    public override void Dispose()
    {

    }
}
