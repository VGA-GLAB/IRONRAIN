using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class EnemyManagerResumeBossActionSequence : ISequence
    {
        [OpenScriptButton(typeof(EnemyManagerResumeBossActionSequence))]
        [Description("ポーズしたボスを再生させるSequence")]
        [Header("このSequenceを抜けるまでの時間(秒)"), SerializeField] private float _totalSec = 0F;

        private EnemyManager _enemyManager;

        public void SetParams(float totalSec)
        {
            _totalSec = totalSec;
        }

        public void SetData(SequenceData data)
        {
            _enemyManager = data.EnemyManager;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            _enemyManager.ResumeBossAction();

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _enemyManager.ResumeBossAction();
        }
    }
}
