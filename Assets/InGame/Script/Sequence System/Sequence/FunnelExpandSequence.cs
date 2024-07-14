using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class FunnelExpandSequence : ISequence
    {
        private EnemyManager _enemyManager;
        
        public void SetData(SequenceData data)
        {
            _enemyManager = data.EnemyManager;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            _enemyManager.FunnelExpand();
        }

        public void Skip()
        {
            _enemyManager.FunnelExpand();
        }
    }
}
