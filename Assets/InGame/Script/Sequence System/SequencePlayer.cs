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

            if (_isSkip)
            {
                for (int i = 0; i < sequences.Length; i++)
                {
                    if (i < _startIndex)
                    {
                        sequences[i].Skip();
                    }
                    else
                    {
                        await sequences[i].PlayAsync(ct);
                    }
                }
            }
            else
            {
                foreach (var seq in sequences)
                {
                    _currentSequence = seq;
                    await _currentSequence.PlayAsync(ct);
                }   
            }
        }
    }
}
