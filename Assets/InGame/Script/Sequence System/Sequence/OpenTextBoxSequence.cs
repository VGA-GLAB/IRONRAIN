using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class OpenTextBoxSequence : ISequence
    {
        private TutorialTextBoxController _tutorialText;

        [OpenScriptButton(typeof(OpenTextBoxSequence))]
        [Description("テキストボックスを開くシーケンス")]
        [Header("このSequenceを抜けるまでの時間(秒)"), SerializeField] private float _totalSec = 0F;
        [Header("開く時間(秒)"), SerializeField] private float _openSec = 0F;
        [Header("開くまでに何秒遅らせるか"), SerializeField] private float _delaySec = 0F;

        public void SetData(SequenceData sequenceData)
        {
            _tutorialText = sequenceData.TextBox;
        }

        public void SetParams(float totalSec, float openSec, float delaySec)
        {
            _totalSec = totalSec;
            _openSec = openSec;
            _delaySec = delaySec;
        }
        
        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            _tutorialText.DoOpenTextBoxAsync(_openSec, ct).Forget(exceptionHandler);

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _tutorialText.Open();
        }
    }
}
