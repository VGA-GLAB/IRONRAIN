using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public sealed class InGameManager : MonoBehaviour
{
    [SerializeField] private bool _isSkip = false;
    [Header("そのインデックスのシーケンスまでスキップします")]
    [SerializeField] private int _skipIndex = 0;
    
    public AbstractSequenceBase CurrentSequence => _currentSequence;

    [SerializeField] private AbstractSequenceBase[] _sequences = default;
    private AbstractSequenceBase _currentSequence;


    public interface IProvidePlayerInformation
    {
        public ISubject<Unit> StartQTE { get; }
        
        public ISubject<QTEResultType> EndQTE { get; }
        
        public float TimeScale { get; set; }
    }

    public void InGameStartUnityEventReceive() => InGameStartAsync(this.GetCancellationTokenOnDestroy()).Forget();

    private async UniTask InGameStartAsync(CancellationToken ct)
    {
        for (int i = 0; i < _sequences.Length; i++)
        {
            if (_isSkip && i <= _skipIndex)
            {
                _sequences[i].OnSkip();
            }
            else
            {
                await _sequences[i].PlaySequenceAsync(ct);
                _currentSequence = _sequences[i];   
            }
        }
    }
}
