using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace IronRain.SequenceSystem
{
    public class WaitPanelTouchSequence : AbstractWaitSequence
    {
        private RadarMap _raderMap;
        
        public override void SetData(SequenceData data)
        {
            base.SetData(data);
            _raderMap = data.RadarMap;
        }

        public override async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // Loop処理
            using var loopCts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, loopCts.Token);

            this.PlayWaitingSequenceAsync(linkedCts.Token, exceptionHandler).Forget();
            
            await _raderMap.WaitTouchPanelAsync(ct);
            
            loopCts.Cancel();
        }


        public override void Skip()
        {
        }
    }
}
