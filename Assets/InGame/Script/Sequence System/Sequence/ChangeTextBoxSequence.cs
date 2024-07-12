using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace IronRain.SequenceSystem
{
    public sealed class ChangeTextBoxSequence : ISequence
    {
        private TutorialTextBoxController _textBox;
        
        [SerializeField] private float _totalSec = 0F;
        [SerializeField] private string _text = "";
        [SerializeField] private float _oneCharDuration = 0F;
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

        public async UniTask PlayAsync(CancellationToken ct)
        {
            _textBox.DoTextChangeAsync(_text, _oneCharDuration, ct).Forget();

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _textBox.ChangeText(_text);
        }
    }
}