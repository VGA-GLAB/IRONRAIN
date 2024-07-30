using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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

            for (int i = 0; i < sequences.Length; i++)
            {
                _currentSequence = sequences[i];
                
                try
                {
                    if (_isSkip && i < _startIndex)
                    {
                        _currentSequence.Skip();
                    }
                    else
                    {
                        await _currentSequence.PlayAsync(ct, x =>ExceptionReceiver(x, i));
                    }
                }
                catch (OperationCanceledException e) { }
                catch (Exception e)
                {
                    ExceptionReceiver(e, i);
                }
            }
        }

        private void ExceptionReceiver(Exception e, int index)
        {
            Debug.LogError($"Sequence Element{index}でエラー");
            throw e;
        }
    }
}
