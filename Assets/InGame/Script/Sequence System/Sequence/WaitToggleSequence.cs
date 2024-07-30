using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    [Serializable]
    public sealed class WaitToggleSequence : ISequence
    {
        [Header("どの入力を待つのか"), SerializeField] private InputProvider.InputType _toggleButton1 = InputProvider.InputType.Toggle1;
        [SerializeField] private InputProvider.InputType _toggleButton2 = InputProvider.InputType.Toggle2;

        public void SetParams(InputProvider.InputType toggleButton1, InputProvider.InputType toggleButton2)
        {
            _toggleButton1 = toggleButton1;
            _toggleButton2 = toggleButton2;
        }
        
        public void SetData(SequenceData data) { }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // 二つの入力を待つ
            await UniTask.WhenAll(
                UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(_toggleButton1), cancellationToken: ct),
                UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(_toggleButton2), cancellationToken: ct)
            );
        }

        public void Skip()
        {
        }
    }
}
