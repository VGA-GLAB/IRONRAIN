using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class SequenceGroup : ISequence
    {
        [SerializeField] private string _groupName;
        [SerializeField] private bool _isSkip = false;
        public string GroupName => _groupName;
        [SerializeReference, SubclassSelector] private ISequence[] _sequences;

        
        public void SetData(SequenceData data)
        {
            foreach (var sec in _sequences)
            {
                sec.SetData(data);
            }
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            if (_isSkip)
            {
                Skip();

                await UniTask.CompletedTask;
            }
            else
            {
                for (int i = 0; i < _sequences.Length; i++)
                {
                    try
                    {
                        var index = i;
                        await _sequences[i]
                            .PlayAsync(ct, exceptionHandler + (x => ExceptionReceiver(x, index)));
                    }
                    catch (OperationCanceledException e) { }
                    catch (Exception e)
                    {
                        ExceptionReceiver(e, i);
                    }
                }
            }
        }

        private void ExceptionReceiver(Exception e, int index)
        {
            Debug.LogError($"{_groupName}の Element{index}");
            throw e;
        }

        public void Skip()
        {
            for (int i = 0; i < _sequences.Length; i++)
            {
                _sequences[i].Skip();
            }
        }
    }
}
