using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class WaitAllDefeatedSequence : ISequence
    {
        [SerializeField] private EnemyManager.Sequence _targetSeq;

        private EnemyManager _enemyManager;

        private void SetParams(EnemyManager.Sequence targetSeq)
        {
            _targetSeq = targetSeq;
        }
        
        public void SetData(SequenceData data)
        {
            _enemyManager = data.EnemyManager;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            await UniTask.WaitUntil(() => _enemyManager.IsAllDefeated(EnemyManager.Sequence.Attack),
                cancellationToken: ct);
        }

        public void Skip() { }
    }
}
