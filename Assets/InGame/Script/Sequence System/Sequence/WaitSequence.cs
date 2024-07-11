using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class WaitSequence : ISequence
    {
        /// <summary>このシーケンス全体の時間</summary>
        [SerializeField] private float _totalSec = 0F;
        
        public void SetParams(float totalSec)
        {
            _totalSec = totalSec;
        }

        public void SetData(SequenceData data) { }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip() { }
    }
}
