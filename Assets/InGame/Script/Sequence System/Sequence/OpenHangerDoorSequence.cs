using System;
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
        
        [Header("このSequenceを抜けるまでの時間(秒)"), SerializeField] private float _totalSec = 0F;
        [Header("どのドアを開けるか"), SerializeField] private TargetDoor _targetDoor;
        
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

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
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
