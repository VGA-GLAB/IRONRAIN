using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class GroupDataSequencer : ISequence
    {
        [SerializeField]
        private SequenceGroupData _groupData;

        public void SetData(SequenceData data) => _groupData.SetData(data);

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            return _groupData.PlaySequence(ct, exceptionHandler);
        }

        public void Skip() => _groupData.Skip();
    }
}
