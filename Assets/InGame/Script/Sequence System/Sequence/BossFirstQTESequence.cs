using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class BossFirstQTESequence : ISequence
    {
        private SequenceData _data;


        public void SetData(SequenceData data)
        {
            _data = data;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            _data.EnemyManager.BossFirstQte();
            await _data.PlayerController.SeachState<PlayerQTE>().QTEModel.StartQTE(Guid.Empty, QteType.BossQte1);
        }

        public void Skip()
        {
        }
    }
}
