using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace IronRain.SequenceSystem
{
    public class WaitPanelTouchSequence : ISequence
    {
        private RaderMap _raderMap;
        
        public void SetData(SequenceData data)
        {
            _raderMap = data.RaderMap;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            await _raderMap.WaitTouchPanelAsync(ct);
        }


        public void Skip()
        {
        }
    }
}
