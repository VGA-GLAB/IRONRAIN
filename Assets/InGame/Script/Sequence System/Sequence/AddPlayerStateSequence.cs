﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using IronRain.Player;

namespace IronRain.SequenceSystem
{
    public sealed class AddPlayerStateSequence : ISequence
    {
        [OpenScriptButton(typeof(AddPlayerStateSequence))]
        [Description("プレイヤーに状態を付与するシーケンスです。\n次のシーケンスに移動するまでの時間と、追加する状態を指定できます。")]
        [Header("このSequenceを抜けるまでの時間(秒)"), SerializeField] private float _totalSec = 0F;
        [Header("Playerに追加するState"), SerializeField] private PlayerStateType _addState;

        private PlayerController _playerController;

        public void SetParams(float totalSec, PlayerStateType addState)
        {
            _totalSec = totalSec;
            _addState = addState;
        }

        public void SetData(SequenceData data)
        {
            _playerController = data.PlayerController;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            _playerController.PlayerEnvroment.AddState(_addState);

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _playerController.PlayerEnvroment.AddState(_addState);
        }
    }
}