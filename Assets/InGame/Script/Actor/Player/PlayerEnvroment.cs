using System.Collections.Generic;
using UnityEngine;

public class PlayerEnvroment
{
    public Transform PlayerTransform { get; private set; }
    public PlayerSetting PlayerSetting { get; private set; }
    public PlayerStateType PlayerState { get; private set; }
    public RaderMap RaderMap { get; private set; }
    public PlayerAnimation PlayerAnimation { get; private set; }
    public TutorialTextBoxController TutorialTextBoxCon { get; private set; }

    private List<PlayerComponentBase> _playerComponentList;

    public PlayerEnvroment(Transform playerTransform, PlayerSetting playerSetting, 
        RaderMap raderMap, List<PlayerComponentBase> playerComponentList,
        PlayerAnimation playerAnimation, TutorialTextBoxController tutorialTextBoxController) 
    {
        PlayerTransform = playerTransform;
        PlayerSetting = playerSetting;
        RaderMap = raderMap;
        _playerComponentList = playerComponentList;
        PlayerAnimation = playerAnimation;
        TutorialTextBoxCon = tutorialTextBoxController;
    }

    /// <summary>
    /// 状態を追加する
    /// </summary>
    /// <param name="state">追加する状態</param>
    public void AddState(PlayerStateType state)
    {
        PlayerState |= state;
    }

    /// <summary>
    /// 状態を削除する
    /// </summary>
    /// <param name="state">削除する状態</param>
    public void RemoveState(PlayerStateType state)
    {
        PlayerState &= ~state;
    }

    /// <summary>
    /// ステートをすべてクリアする
    /// </summary>
    public void ClearState() 
    {
        PlayerState &= 0;
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
