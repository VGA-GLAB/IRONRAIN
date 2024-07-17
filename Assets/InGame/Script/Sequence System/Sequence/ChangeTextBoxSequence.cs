using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class ChangeTextBoxSequence : ISequence
    {
        private TutorialTextBoxController _textBox;
        
        [SerializeField] private float _totalSec = 0F;
        [SerializeField,TextArea] private string _text = "";
        [SerializeField] private float _oneCharDuration = 0.05F;
        [SerializeField] private float _delaySec = 0F;
        
        public void SetData(SequenceData data)
        {
            _textBox = data.TextBox;
        }

        public void SetParams(float totalSec, string text, float changeSec, float delaySec)
        {
            _totalSec = totalSec;
            _text = text;
            _oneCharDuration = changeSec;
            _delaySec = delaySec;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            _textBox.DoTextChangeAsync(_text, _oneCharDuration, ct)
                .Forget(exceptionHandler);

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _textBox.ChangeText(_text);
        }
    }
}
