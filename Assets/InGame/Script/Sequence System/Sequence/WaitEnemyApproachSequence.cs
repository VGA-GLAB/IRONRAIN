using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Enemy;

namespace IronRain.SequenceSystem
{
    public sealed class WaitEnemyApproachSequence : ISequence
    {
        [OpenScriptButton(typeof(WaitEnemyApproachSequence))]
        [Description("対象のSequenceの敵が近づくのを待つSequence")]
        [SerializeField, Header("対象のSequenceのID")]
        private int _sequenceId = 0;

        private List<EnemyController> _targetEnemies = new();
        
        public void SetData(SequenceData data)
        {
            data.EnemyManager.TryGetEnemies(_sequenceId, _targetEnemies);
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            if (_targetEnemies is null || _targetEnemies.Count == 0) 
            {
                await UniTask.CompletedTask;
                return;
            }
            var enemyApproachAsync = new UniTask[_targetEnemies.Count];

            for (int i = 0; i < _targetEnemies.Count; i++)
            {
                var target = _targetEnemies[i];
                enemyApproachAsync[i] = UniTask.WaitUntil(
                    () => target.BlackBoard.IsApproachCompleted,
                    cancellationToken: ct
                    );
            }

            await UniTask.WhenAll(enemyApproachAsync);
        }

        public void Skip() { }
    }
}
