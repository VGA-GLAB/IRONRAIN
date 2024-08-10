using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using IronRain.Player;

namespace IronRain.SequenceSystem
{
    public sealed class BossStartSequence : ISequence
    {
        private SequenceData _data;

        public void SetData(SequenceData data)
        {
            _data = data;
        }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // BossStart処理
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