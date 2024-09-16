using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    [Serializable]
    public abstract class AbstractWaitSequence : ISequence
    {
        [SerializeReference, SubclassSelector, Header("待機中のループ処理")]
        private ISequence[] _waitSequences;

        public virtual void SetData(SequenceData data)
        {
            for (int i = 0; i < _waitSequences.Length; i++)
            {
                _waitSequences[i].SetData(data);
            }
        }

        public abstract UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null);

        public abstract void Skip();

        /// <summary>待機中のループ処理</summary>
        protected virtual async UniTaskVoid PlayWaitingSequenceAsync(CancellationToken ct, Action<Exception> exceptionHandler)
        {
            var index = 0;
            // Cancelされたらループを抜ける
            while (!ct.IsCancellationRequested && _waitSequences.Length > 0)
            {
                await _waitSequences[index].PlayAsync(ct, exceptionHandler);
                index++;

                if (index >= _waitSequences.Length)
                {
                    index = 0;
                }
            }
        }
    }
}
