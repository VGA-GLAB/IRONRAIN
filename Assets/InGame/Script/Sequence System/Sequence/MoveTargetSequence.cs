using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class MoveTargetSequence : ISequence
    {
        public enum MoveTarget
        {
            SecondDoor,
            Outside
        }
        
        [SerializeField] private float _totalSec = 0F;
        [SerializeField] private float _moveSec = 0F;

        private SequenceData _data;
        
        public void SetParams(float totalSec, float moveSec)
        {
            _totalSec = totalSec;
            _moveSec = moveSec;
        }
        
        public void SetData(SequenceData data)
        {
            _data = data;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            
        }

        private async UniTask MoveTarget(CancellationToken ct)
        {
            
        }

        public void Skip()
        {
            
        }
    }
}
