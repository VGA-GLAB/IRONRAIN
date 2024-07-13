using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class BossStartSequence : ISequence
    {
        private SequenceData _data;
        
        public void SetData(SequenceData data)
        {
            _data = data;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            _data.PlayerController.SeachState<PlayerStoryEvent>().BossStart();
            _data.EnemyManager.BossStart();
        }

        public void Skip()
        {
            _data.PlayerController.SeachState<PlayerStoryEvent>().BossStart();
            _data.EnemyManager.BossStart();
        }
    }
}
