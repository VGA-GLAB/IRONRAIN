using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class CloseTextBoxSequence : ISequence
    {
        [Header("このSequenceを抜けるまでの時間(秒)"), SerializeField] private float _totalSec = 0f;
        [Header("テキストボックスを閉じる時間(秒)"), SerializeField] private float _closeDuration = 1F;

        private SequenceData _data;
        
        public void SetData(SequenceData data)
        {
            _data = data;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            _data.TextBox.DoCloseTextBoxAsync(_closeDuration, ct)
                .Forget(exceptionHandler);

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _data.TextBox.Close();
        }
    }
}
