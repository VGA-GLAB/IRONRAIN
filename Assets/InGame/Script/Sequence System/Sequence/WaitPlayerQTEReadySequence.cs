using Cysharp.Threading.Tasks;
using IronRain.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class WaitPlayerQTEReadySequence : ISequence
    {
        private PlayerQTE _playerQTE;

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // Shieldの敵が接近してくるのを待っている
            await UniTask.WaitUntil(() => _playerQTE.IsEnterShield, cancellationToken: ct);
        }

        public void SetData(SequenceData data)
        {
            _playerQTE = data.PlayerController.SeachState<PlayerQTE>();
        }

        public void Skip() { }
    }
}
