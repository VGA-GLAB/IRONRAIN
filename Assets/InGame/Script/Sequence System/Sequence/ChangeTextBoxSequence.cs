using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class ChangeTextBoxSequence : ISequence
    {
        private TutorialTextBoxController _textBox;
        
        [Header("このSequenceを抜けるまでの時間(秒)"), SerializeField] private float _totalSec = 0F;
        [Header("文字を更新する前に文字を消すか"), SerializeField] private bool _clearText = true;
        [Header("Text"), SerializeField, TextArea] private string _text = "";
        [Header("1文字を出力する時間(秒)"), SerializeField] private float _oneCharDuration = 0.05F;
        [Header("文字の更新の開始をどのくらい遅らせるか(秒)"), SerializeField] private float _delaySec = 0F;
        
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
            if (_clearText) _textBox.ClearText();
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
