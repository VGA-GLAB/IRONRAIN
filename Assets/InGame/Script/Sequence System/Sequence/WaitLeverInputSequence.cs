using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class WaitLeverInputSequence : ISequence
    {
        public void SetData(SequenceData data) { }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            await UniTask.WaitUntil(() =>
                InputProvider.Instance.LeftLeverDir != Vector3.zero ||
                InputProvider.Instance.RightLeverDir != Vector3.zero);
        }

        public void Skip() { }
    }
}
