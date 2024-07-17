using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor.VersionControl;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class SequencePlayer : MonoBehaviour
    {
        public ISequence CurrentSequence => _currentSequence;

        [SerializeField] private SequenceManager _manager;
        [SerializeField] private bool _playOnStart = true;
        [Header("スキップ機能")]
        [SerializeField] private bool _isSkip = false;
        [SerializeField] private int _startIndex;

        private ISequence[] _sequences;
        private ISequence _currentSequence;
        private static int _index;

        private void Start()
        {
            _sequences = _manager.GetSequences();

            if (_playOnStart)
            {
                Play();
            }
        }

        public void Play() => PlayAsync(this.GetCancellationTokenOnDestroy()).Forget();
        
        public async UniTask PlayAsync(CancellationToken ct)
        {
            var sequences = _manager.GetSequences();

            for (_index = 0; _index < sequences.Length; _index++)
            {
                _currentSequence = sequences[_index];
                
                try
                {
                    if (_isSkip && _startIndex < _index)
                    {
                        _currentSequence.Skip();
                    }
                    else
                    {
                        await _currentSequence.PlayAsync(ct);
                    }
                }
                catch (Exception e)
                {
                    SequencePlayerExceptionReceiver(e);
                }
            }
        }

        public static void SequencePlayerExceptionReceiver(Exception e)
        {
            Debug.LogError($"Sequence Element{_index}でエラー");
            throw e;
        }
    }
}
