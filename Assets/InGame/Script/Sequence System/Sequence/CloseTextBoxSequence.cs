using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class CloseTextBoxSequence : ISequence
    {
        [SerializeField] private float _totalSec = 0f;
        [SerializeField] private float _closeDuration = 1F;

        private SequenceData _data;
        
        public void SetData(SequenceData data)
        {
            _data = data;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            _data.TextBox.DoCloseTextBoxAsync(_closeDuration, ct)
                .Forget(SequencePlayer.SequencePlayerExceptionReceiver);

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _data.TextBox.Close();
        }
    }
}
