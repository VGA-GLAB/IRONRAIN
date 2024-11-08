using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace IronRain.SequenceSystem
{
    public class WaitMultiLockSequence : AbstractWaitSequence
    {
        private LockOnSystem _lockSystem;
        private RadarMap _raderMap;
        
        public override void SetData(SequenceData data)
        {
            base.SetData(data);
            _lockSystem = data.LockSystem;
            _raderMap = data.RaderMap;
        }

        public override async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            using var loopCts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, loopCts.Token);

            this.PlayWaitingSequenceAsync(linkedCts.Token, exceptionHandler).Forget();
            
            var enemy = await _lockSystem.MultiLockOnAsync(ct);
            _raderMap.MultiLockOn(enemy);

            loopCts.Cancel();
        }

        public override void Skip() { }
    }
}
