using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace IronRain.SequenceSystem
{
    public class BossSecondQTESequence : ISequence
    {
        private SequenceData _data;


        public void SetData(SequenceData data)
        {
            _data = data;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            _data.EnemyManager.BossSecondQte();
            await _data.PlayerController.SeachState<PlayerQTE>().QTEModel.StartQTE(Guid.Empty, QteType.BossQte2);
        }

        public void Skip()
        {
            throw new System.NotImplementedException();
        }
    }
}