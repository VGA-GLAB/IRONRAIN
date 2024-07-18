using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace IronRain.SequenceSystem
{
    public class WaitMultiLockSequence : ISequence
    {
        private LockOnSystem _lockSystem;
        private RaderMap _raderMap;
        
        public void SetData(SequenceData data)
        {
            _lockSystem = data.LockSystem;
            _raderMap = data.RaderMap;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            var enemy = await _lockSystem.MultiLockOnAsync(ct);
            _raderMap.MultiLockon(enemy);
        }

        public void Skip() { }
    }
}
