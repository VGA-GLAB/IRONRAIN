using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;

namespace IronRain.SequenceSystem
{
    public sealed class FunnelExpandSequence : ISequence
    {
        private EnemyManager _enemyManager;
        
        public void SetData(SequenceData data)
        {
            _enemyManager = data.EnemyManager;
        }

        public UniTask PlayAsync(CancellationToken ct)
        {
            _enemyManager.FunnelExpand();

            return UniTask.CompletedTask;
        }

        public void Skip()
        {
            _enemyManager.FunnelExpand();
        }
    }
}