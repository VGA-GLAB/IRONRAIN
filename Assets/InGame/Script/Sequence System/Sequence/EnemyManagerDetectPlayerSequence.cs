using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class EnemyManagerDetectPlayerSequence : ISequence
    {
        [Header("このSequenceを抜けるまでの時間(秒)"), SerializeField] private float _totalSec = 0F;
        [Header("どのSequenceの敵を出現させるのか"), SerializeField] private EnemyManager.Sequence _targetSeq;

        private EnemyManager _enemyManager;
        
        public void SetParams(float totalSec, EnemyManager.Sequence targetSeq)
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
            _enemyManager.DetectPlayer(_targetSeq);

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _enemyManager.DetectPlayer(_targetSeq);
        }
    }
}
