using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public sealed class InGameManager : MonoBehaviour
{
    public AbstractSequenceBase CurrentSequence => _currentSequence;

    [SerializeField] private AbstractSequenceBase[] _sequences = default;
    private AbstractSequenceBase _currentSequence;


    public interface IProvidePlayerInformation
    {
        public ISubject<Unit> StartQTE { get; }
        
        public ISubject<QTEResultType> EndQTE { get; }
        
        public float TimeScale { get; set; }
    }

    public async void InGameStart()
    {
        foreach (var seq in _sequences)
        {
            await seq.PlaySequenceAsync(this.GetCancellationTokenOnDestroy());
            _currentSequence = seq;
        }
    }
}
