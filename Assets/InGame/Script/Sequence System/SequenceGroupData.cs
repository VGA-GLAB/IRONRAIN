using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    [CreateAssetMenu(fileName = "GroupData", menuName = "SequenceSystem/SequenceGroupData")]
    public sealed class SequenceGroupData : ScriptableObject
    {
        [Description("処理ごとにSequenceをAssetにまとめておく機能")]
        [SerializeReference, SubclassSelector]
        private ISequence[] _sequences;

        public void SetData(SequenceData data)
        {
            for (int i = 0; i < _sequences.Length; i++)
            {
                try
                {
                    _sequences[i].SetData(data);
                }
                catch (Exception e)
                {
                    ExceptionReceiver(e, i);
                }
            }
        }

        /// <summary>まとめて再生する関数</summary>
        public async UniTask PlaySequence(CancellationToken ct, Action<Exception> exceptionHandler)
        {
            for (int i = 0; i < _sequences.Length; i++)
            {
                try
                {
                    var index = i;
                    await _sequences[i]
                        .PlayAsync(ct, exceptionHandler + (x => ExceptionReceiver(x, index)));
                }
                catch (Exception e)
                {
                    ExceptionReceiver(e, i);
                }
            }
        }
        
        private void ExceptionReceiver(Exception e, int index)
        {
            Debug.LogError($"{name}の Element{index}");
            throw e;
        }

        /// <summary>スキップする関数</summary>
        public void Skip()
        {
            for (int i = 0; i < _sequences.Length; i++)
            {
                try
                {
                    _sequences[i].Skip();
                }
                catch (Exception e)
                {
                    ExceptionReceiver(e, i);
                }
            }
        }
    }
}
