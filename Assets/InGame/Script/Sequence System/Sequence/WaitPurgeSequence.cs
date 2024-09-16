using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using IronRain.Player;

namespace IronRain.SequenceSystem
{
    public sealed class WaitPurgeSequence : AbstractWaitSequence
    {
        private PlayerController _playerController;
        
        public override void SetData(SequenceData data)
        {
            base.SetData(data);
            _playerController = data.PlayerController;
        }

        public override async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // Loop処理
            using var loopCts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, loopCts.Token);

            this.PlayWaitingSequenceAsync(linkedCts.Token, exceptionHandler).Forget();
            
            await _playerController.SeachState<PlayerStoryEvent>().StartJetPackPurge();
            
            loopCts.Cancel();
        }

        public override void Skip() { }
    }
}
