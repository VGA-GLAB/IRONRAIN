﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class EnemyManagerDetectPlayerSequence : ISequence
    {
        [OpenScriptButton(typeof(EnemyManagerDetectPlayerSequence))]
        [Description("対象のシーケンスの敵を出現させるシーケンスです。")]
        [Header("スキップした際に敵を出現させるかどうか"), SerializeField] private bool _isSkipSpawn = false;
        [Header("このSequenceを抜けるまでの時間(秒)"), SerializeField] private float _totalSec = 0F;
        [Header("敵を出現させる対象のID(整数)"), SerializeField] private int _targetSeq;

        private EnemyManager _enemyManager;
        
        public void SetParams(float totalSec, int targetSeq)
        {
            _totalSec = totalSec;
            _targetSeq = targetSeq;
        }
        
        public void SetData(SequenceData data)
        {
            _enemyManager = data.EnemyManager;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            _enemyManager.Spawn(_targetSeq);

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            if (_isSkipSpawn)
            {
                _enemyManager.Spawn(_targetSeq);
            }
        }
    }
}
