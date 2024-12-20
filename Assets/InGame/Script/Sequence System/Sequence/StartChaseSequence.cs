﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using IronRain.Player;

namespace IronRain.SequenceSystem
{
    public class StartChaseSequence : ISequence
    {
        [OpenScriptButton(typeof(StartChaseSequence))]
        [Description("格納庫から発射して、敵を追いかける際の初期化処理を行うシーケンス")]
        [Header("このSequenceを抜けるまでの時間(秒)"), SerializeField] private float _totalSec = 0F;
        private PlayerStoryEvent _playerStoryEvent;

        public void SetParams(float totalSec)
        {
            _totalSec = totalSec;
        }
        
        public void SetData(SequenceData data)
        {
            _playerStoryEvent = data.PlayerStoryEvent;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            _playerStoryEvent.StartChaseScene();

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _playerStoryEvent.StartChaseScene();
        }
    }
}
