using System.Threading;
using Cysharp.Threading.Tasks;

namespace IronRain.SequenceSystem
{
    public class MultiLockOnStartSequence : ISequence
    {
        private MouseMultilockSystem _mouseMultiLockSystem;
        
        public void SetData(SequenceData data)
        {
            _mouseMultiLockSystem = data.MouseMultiLockSystem;
        }

        public UniTask PlayAsync(CancellationToken ct)
        {
            _mouseMultiLockSystem.MultilockOnStart();

            return UniTask.CompletedTask;
        }

        public void Skip()
        {
            _mouseMultiLockSystem.MultilockOnStart();
        }
    }
}
