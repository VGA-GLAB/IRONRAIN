using System.Threading;
using Cysharp.Threading.Tasks;

namespace IronRain.SequenceSystem
{
    public sealed class WaitPurgeSequence : ISequence
    {
        private PlayerController _playerController;
        
        public void SetData(SequenceData data)
        {
            _playerController = data.PlayerController;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            await _playerController.SeachState<PlayerStoryEvent>().StartJetPackPurge();
        }

        public void Skip() { }
    }
}
