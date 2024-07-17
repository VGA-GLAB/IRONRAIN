using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class StartChaseSequence : ISequence
    {
        [SerializeField] private float _totalSec = 0F;
        private PlayerStoryEvent _playerStoryEvent;

        public void SetParams(float totalSec)
        {
            _totalSec = totalSec;
        }
        
        public void SetData(SequenceData data)
        {
            _playerStoryEvent = data.PlayerStoryEvent;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            _playerStoryEvent.StartChaseScene();

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _playerStoryEvent.StartChaseScene();
        }
    }
}
