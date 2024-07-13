using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class MultiLockOnStartSequence : ISequence
    {
        private MouseMultilockSystem _mouseMultiLockSystem;
        
        public void SetData(SequenceData data)
        {
            _mouseMultiLockSystem = data.MouseMultiLockSystem;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            _mouseMultiLockSystem.MultilockOnStart();
        }

        public void Skip()
        {
            _mouseMultiLockSystem.MultilockOnStart();
        }
    }
}
