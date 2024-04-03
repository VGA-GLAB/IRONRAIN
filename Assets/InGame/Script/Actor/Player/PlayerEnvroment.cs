using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnvroment
{
    public Transform PlayerTransform => _playerTransform;
    public PlayerSetting PlayerSetting => _setting;
    public PlayerStateType PlayerState => _playerState;

    private PlayerSetting _setting;
    private Transform _playerTransform;
    private PlayerStateType _playerState;

    public PlayerEnvroment(Transform playerTransform, PlayerSetting playerSetting) 
    {
        _playerTransform = playerTransform;
        _setting = playerSetting;
    }

    /// <summary>
    /// 状態を追加する
    /// </summary>
    /// <param name="state">追加する状態</param>
    public void AddState(PlayerStateType state)
    {
        _playerState |= state;
    }

    /// <summary>
    /// 状態を削除する
    /// </summary>
    /// <param name="state">削除する状態</param>
    public void RemoveState(PlayerStateType state)
    {
        _playerState &= ~state;
    }
}
