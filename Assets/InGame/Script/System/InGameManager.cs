using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public sealed class InGameManager : MonoBehaviour
{
    [SerializeField] private AbstractSequenceBase[] _sequences = default;
    
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
        }
    }
}
