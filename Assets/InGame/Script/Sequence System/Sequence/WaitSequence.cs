using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Oculus.Interaction;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    [Serializable]
    public sealed class WaitSequence : ISequence
    {
        /// <summary>このシーケンス全体の時間</summary>
        [OpenScriptButton(typeof(WaitSequence))]
        [Description("進行を待つためのシーケンスです。\n何秒間待つのかを指定できます。")]
        [Header("このSequenceを抜けるまでの時間(秒)", order = 1), SerializeField] private float _totalSec = 0F;
        
        public void SetParams(float totalSec)
        {
            _totalSec = totalSec;
        }

        public void SetData(SequenceData data) { }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip() { }
    }
}
