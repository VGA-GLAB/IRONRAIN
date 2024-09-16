using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class EnemyManagerPauseOnPlayerForwardSequence : ISequence
    {
        private EnemyManager _enemyManager;
        
        public void SetData(SequenceData data)
        {
            _enemyManager = data.EnemyManager;
        }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            _enemyManager.PauseOnPlayerForward();

            return UniTask.CompletedTask;
        }

        public void Skip()
        {
            _enemyManager.PauseOnPlayerForward();
        }
    }
}
