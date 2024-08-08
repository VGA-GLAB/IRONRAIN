using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class WaitAllDefeatedSequence : ISequence
    {
        [Header("どのSequenceの敵の撃破を待つのか"), SerializeField] private EnemyManager.Sequence _targetSeq;

        private EnemyManager _enemyManager;

        private void SetParams(EnemyManager.Sequence targetSeq)
        {
            _targetSeq = targetSeq;
        }
        
        public void SetData(SequenceData data)
        {
            _enemyManager = data.EnemyManager;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            await UniTask.WaitUntil(() => _enemyManager.IsAllDefeated(_targetSeq),
                cancellationToken: ct);
        }

        public void Skip() { }
    }
}
