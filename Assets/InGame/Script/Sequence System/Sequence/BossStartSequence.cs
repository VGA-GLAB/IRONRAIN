using System.Threading;
using Cysharp.Threading.Tasks;

namespace IronRain.SequenceSystem
{
    public sealed class BossStartSequence : ISequence
    {
        private SequenceData _data;

        public void SetData(SequenceData data)
        {
            _data = data;
        }

        public UniTask PlayAsync(CancellationToken ct)
        {
            _data.PlayerController.SeachState<PlayerStoryEvent>().BossStart();
            _data.EnemyManager.BossStart();

            return UniTask.CompletedTask;
        }

        public void Skip()
        {
            _data.PlayerController.SeachState<PlayerStoryEvent>().BossStart();
            _data.EnemyManager.BossStart();
        }
    }
}