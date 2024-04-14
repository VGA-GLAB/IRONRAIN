using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private List<PlayerStateBase> _playerStateList = new();
    [SerializeField] private PlayerSetting _playerSetting;

    private PlayerEnvroment _playerEnvroment;

    private void Awake()
    {
        _playerEnvroment = new PlayerEnvroment(transform, _playerSetting);
    }

    private void Start()
    {
        for (int i = 0; i < _playerStateList.Count; i++) 
        {
            _playerStateList[i].SetUp(_playerEnvroment, this.GetCancellationTokenOnDestroy());
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _playerStateList.Count; i++)
        {
            _playerStateList[i].Dispose();
        }
    }

    /// <summary>
    /// PlayerStateを検索して返す
    /// </summary>
    /// <typeparam name="T">検索されたState</typeparam>
    /// <returns></returns>
    public T SeachState<T>() where T : class
    {
        for (int i = 0; i < _playerStateList.Count; i++)
        {
            if (_playerStateList[i] is T)
            {
                return _playerStateList[i] as T;
            }
        }
        Debug.LogError("指定されたステートが見つかりませんでした");
        return default;
    }



}
