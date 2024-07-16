using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class SequencePlayer : MonoBehaviour
    {
        public ISequence CurrentSequence => _currentSequence;

        [SerializeField] private SequenceManager _manager;
        [SerializeField] private bool _playOnAwake = true;

        private ISequence[] _sequences;
        private ISequence _currentSequence;

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
                _currentSequence = seq;
                await _currentSequence.PlayAsync(ct);
            }
        }
    }
}
