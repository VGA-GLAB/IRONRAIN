using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using IronRain.Player;

namespace IronRain.SequenceSystem
{
    public sealed class WaitStartFallSequence : ISequence
    {
        private PlayerController _playerController;
        private RadarMap _raderMap;
        
        public void SetData(SequenceData data)
        {
            _playerController = data.PlayerController;
            _raderMap = data.RaderMap;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // UIの処理をする
            _raderMap.StartPurgeSequence();
            await _playerController.SeachState<PlayerStoryEvent>().StartFall();
            _raderMap.EndPurgeSequence();
        }

        public void Skip() { }
    }
}
