using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace IronRain.SequenceSystem
{
    public class WaitMultiLockSequence : ISequence
    {
        private MultilockSystemExample _multiLockSystem;
        private RaderMap _raderMap;
        
        public void SetData(SequenceData data)
        {
            _multiLockSystem = data.MultiLockSystem;
            _raderMap = data.RaderMap;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            var enemy = await _multiLockSystem.LockOnAsync(ct);
            _raderMap.MultiLockon(enemy);
        }

        public void Skip() { }
    }
}
