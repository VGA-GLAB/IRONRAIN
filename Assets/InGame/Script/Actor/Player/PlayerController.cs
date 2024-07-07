﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class PlayerController : MonoBehaviour
{
    public PlayerEnvroment PlayerEnvroment => _playerEnvroment;

    [SerializeField] private List<PlayerComponentBase> _playerStateList = new();
    [SerializeField] private PlayerSetting _playerSetting;
    [SerializeField] private RaderMap _playerMap;
    [SerializeField] private TutorialTextBoxController _tutorialTextBoxController;
    [SerializeField] private PlayerAnimation _playerAnimation;

    private PlayerEnvroment _playerEnvroment;

    private void Awake()
    {
        _playerEnvroment = new PlayerEnvroment(transform, _playerSetting, _playerMap, 
            _playerStateList, _playerAnimation, _tutorialTextBoxController);

        for (int i = 0; i < _playerStateList.Count; i++)
        {
            _playerStateList[i].SetUp(_playerEnvroment, this.GetCancellationTokenOnDestroy());
        }

        if (_playerSetting.IsStartChaseScene) 
        {
            SeachState<PlayerStoryEvent>().StartChaseScene();
        }

        if (_playerSetting.IsToggleTest) 
        {
            InputProvider.Instance.SetEnterInput(InputProvider.InputType.OneButton, () =>
            {
                InputProvider.Instance.CallEnterInput(InputProvider.InputType.Toggle1);
                InputProvider.Instance.CallEnterInput(InputProvider.InputType.Toggle2);
                InputProvider.Instance.CallEnterInput(InputProvider.InputType.Toggle3);
                InputProvider.Instance.CallEnterInput(InputProvider.InputType.Toggle4);
                InputProvider.Instance.CallEnterInput(InputProvider.InputType.Toggle5);
                InputProvider.Instance.CallEnterInput(InputProvider.InputType.Toggle6);
            });
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
    public T SeachState<T>() where T : PlayerComponentBase
    {
       return _playerEnvroment.SeachState<T>();
    }
}
