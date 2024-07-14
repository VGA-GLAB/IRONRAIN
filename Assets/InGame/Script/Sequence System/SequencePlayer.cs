using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class SequencePlayer : MonoBehaviour
    {
        [SerializeField] private SequenceManager _manager;
        [SerializeField] private bool _playOnAwake = true;

        private ISequence[] _sequences;

        private void Awake()
        {
            _sequences = _manager.GetSequences();

            if (_playOnAwake)
            {
                Play();
            }
        }

        public void Play() => PlayAsync(this.GetCancellationTokenOnDestroy()).Forget();
        
        public async UniTask PlayAsync(CancellationToken ct)
        {
            foreach (var seq in _manager.GetSequences())
            {
                await seq.PlayAsync(ct);
            }
        }
    }
}
