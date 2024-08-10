using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using IronRain.Player;

namespace IronRain.SequenceSystem
{
    public class BossSecondQTESequence : ISequence
    {
        private SequenceData _data;
        
        public void SetData(SequenceData data)
        {
            _data = data;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            _data.EnemyManager.BossSecondQte();
            
            await _data.PlayerController.SeachState<PlayerQTE>().QTEModel.StartQTE(Guid.Empty, QteType.BossQte2);
        }

        public void Skip() { }
    }
}
