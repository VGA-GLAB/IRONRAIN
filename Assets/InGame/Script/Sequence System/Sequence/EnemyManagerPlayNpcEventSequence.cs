using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class EnemyManagerPlayNpcEventSequence : ISequence
    {
        [OpenScriptButton(typeof(EnemyManagerPlayNpcEventSequence))]
        [Description("対象のシーケンスの敵のNpcEventを呼ぶシーケンス")]
        [Header("このSequenceを抜けるまでの時間(秒)"), SerializeField] private float _totalSec = 0F;
        [Header("対象のSequenceのID"), SerializeField] private int _targetSeq;

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
            _enemyManager.PlayNpcEvent(_targetSeq);

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _enemyManager.PlayNpcEvent(_targetSeq);
        }
    }
}
