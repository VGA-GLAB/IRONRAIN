using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class SequenceGroup : ISequence
    {
        [Header("グループ名"), SerializeField] private string _groupName;
        [Header("スキップするか"), SerializeField] private bool _isSkip = false;
        public string GroupName => _groupName;
        [SerializeReference, SubclassSelector] private ISequence[] _sequences;

        
        public void SetData(SequenceData data)
        {
            for (int i = 0; i < _sequences.Length; i++)
            {
                try
                {
                    _sequences[i].SetData(data);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    ExceptionReceiver(e, i);
                }
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
                        Debug.Log($"SequenceChange {i}");
                        await _sequences[i]
                            .PlayAsync(ct, exceptionHandler + (ex => ExceptionReceiver(ex, i)));
                    }
                    catch (Exception e) when (e is not OperationCanceledException)
                    {
                        exceptionHandler?.Invoke(e);
                    }
                }
            }
        }

        private void ExceptionReceiver(Exception e, int index)
        {
            if (e is OperationCanceledException)
            {
                // キャンセル例外は無視
                return;
            }

            Debug.LogError($"{_groupName}の Element{index}でエラーが発生 : {e.Message}");
        }

        public void Skip()
        {
            for (int i = 0; i < _sequences.Length; i++)
            {
                try
                {
                    _sequences[i].Skip();
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    ExceptionReceiver(e, i);
                }
            }
        }
    }
}
