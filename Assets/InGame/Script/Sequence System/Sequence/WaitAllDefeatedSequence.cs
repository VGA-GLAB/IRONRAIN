using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class WaitAllDefeatedSequence : ISequence
    {
        [OpenScriptButton(typeof(WaitAllDefeatedSequence))]
        [Description("指定したシーケンスの敵がすべて破壊されるまで、つぎのシーケンスに遷移するのを待つシーケンス")]
        [Header("対象のSequenceのID"), SerializeField] private int _targetSeq;

        private EnemyManager _enemyManager;

        private void SetParams(int targetSeq)
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
