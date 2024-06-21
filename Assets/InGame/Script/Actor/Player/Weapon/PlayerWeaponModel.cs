using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponModel : IPlayerStateModel
{
    public PlayerWeaponBase CurrentWeapon => _playerWeaponList[_currentWeaponIndex];

    [SerializeField] private List<PlayerWeaponBase> _playerWeaponList = new();

    private bool _isWeaponChenge;
    private bool _isShot;
    //0始まり
    private int _currentWeaponIndex;
    private PlayerEnvroment _playerEnvroment;
    private CancellationToken _rootCancellOnDestroy;

    public void SetUp(PlayerEnvroment env, CancellationToken token)
    {
        _playerEnvroment = env;
        _rootCancellOnDestroy = token;
    }

    public void Start()
    {
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.WeaponChenge, WeaponChenge);
        for (int i = 0; i < _playerWeaponList.Count; i++)
        {
            _playerWeaponList[i].SetUp(_playerEnvroment);
        }
    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {
        Shot();
    }

    /// <summary>
    /// 武器切り替え
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
        Debug.Log($"現在の武器は{_playerWeaponList[_currentWeaponIndex]}");
    }

    private void Shot()
    {
        _isShot = InputProvider.Instance.GetStayInput(InputProvider.InputType.OneButton);
        if (_playerEnvroment.PlayerState.HasFlag(PlayerStateType.SwitchingArms)
            || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.RepairMode)
            || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE)
            || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.NonAttack)
            || !_isShot) return;
        
        _playerWeaponList[_currentWeaponIndex].Shot();
    }

    public void Dispose()
    {

    }
}
