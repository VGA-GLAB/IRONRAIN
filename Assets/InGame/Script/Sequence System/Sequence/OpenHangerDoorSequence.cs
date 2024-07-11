using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    [Serializable]
    public class OpenHangerDoorSequence : ISequence
    {
        [Serializable]
        public enum TargetDoor
        {
            First,
            Second
        }
        
        [SerializeField] private float _totalSec = 0F;
        [SerializeField] private TargetDoor _targetDoor;
        
        private HatchController _hatch;
        private SequenceData _data;

        public void SetParams(float totalSec, TargetDoor target)
        {
            _totalSec = totalSec;
            _targetDoor = target;
        }
        
        public void SetData(SequenceData data)
        {
            _data = data;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            if (_targetDoor == TargetDoor.First)
            {
                _data.FirstHatch.Open();
            }
            else
            {
                _data.SecondHatch.Open();
            }

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            if (_targetDoor == TargetDoor.First)
            {
                _data.FirstHatch.Open();
            }
            else
            {
                _data.SecondHatch.Open();
            }
        }
    }
}
