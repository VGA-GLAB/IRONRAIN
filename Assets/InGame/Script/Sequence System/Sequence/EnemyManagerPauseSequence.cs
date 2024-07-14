using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class EnemyManagerPauseSequence : ISequence
    {
        [SerializeField] private float _totalSec = 0F;
        [SerializeField] private EnemyManager.Sequence _targetSeq;

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

        public async UniTask PlayAsync(CancellationToken ct)
        {
            _enemyManager.Pause(_targetSeq);

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _enemyManager.Pause(_targetSeq);
        }
    }
}
