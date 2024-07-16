using System.Threading;
using Cysharp.Threading.Tasks;

namespace IronRain.SequenceSystem
{
    public class WaitMultiLockSequence : ISequence
    {
        private MouseMultilockSystem _mouseMultiLockSystem;
        
        public void SetData(SequenceData data)
        {
            _mouseMultiLockSystem = data.MouseMultiLockSystem;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            await UniTask.WaitUntil(() => !_mouseMultiLockSystem.IsMultilock, cancellationToken: ct);
        }

        public void Skip() { }
    }
}
