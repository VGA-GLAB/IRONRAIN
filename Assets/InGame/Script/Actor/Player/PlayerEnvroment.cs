using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnvroment
{
    public Transform PlayerTransform => _playerTransform;
    public PlayerSetting PlayerSetting => _setting;
    public PlayerStateType PlayerState => _playerState;
    public RaderMap RaderMap => _raderMap;
    public PlayerAnimation PlayerAnimation => _playerAnimation;
    public List<PlayerComponentBase> playerComponentList => _playerComponentList;

    private PlayerSetting _setting;
    private Transform _playerTransform;
    private PlayerStateType _playerState;
    private RaderMap _raderMap;
    private List<PlayerComponentBase> _playerComponentList;
    private PlayerAnimation _playerAnimation;

    public PlayerEnvroment(Transform playerTransform, PlayerSetting playerSetting, 
        RaderMap raderMap, List<PlayerComponentBase> playerComponentList,
        PlayerAnimation playerAnimation) 
    {
        _playerTransform = playerTransform;
        _setting = playerSetting;
        _raderMap = raderMap;
        _playerComponentList = playerComponentList;
        _playerAnimation = playerAnimation;
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

    /// <summary>
    /// ステートをすべてクリアする
    /// </summary>
    public void ClearState() 
    {
        _playerState &= 0;
    }

    /// <summary>
    /// PlayerStateを検索して返す
    /// </summary>
    /// <typeparam name="T">検索されたState</typeparam>
    /// <returns></returns>
    public T SeachState<T>() where T : class
    {
        for (int i = 0; i < _playerComponentList.Count; i++)
        {
            if (_playerComponentList[i] is T)
            {
                return _playerComponentList[i] as T;
            }
        }
        Debug.LogError("指定されたステートが見つかりませんでした");
        return default;
    }
}
