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
        [Header("どのSequenceの敵のイベントを呼び出すのか"), SerializeField] private EnemyManager.Sequence _targetSeq;

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
            _enemyManager.PlayNpcEvent(_targetSeq);

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _enemyManager.PlayNpcEvent(_targetSeq);
        }
    }
}
