using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class StartUpSequence : ISequence
    {
        /// <summary>シーケンス全体の時間</summary>
        [SerializeField] private float _totalSec = 0f;
        /// <summary>イベント番号</summary>
        [SerializeField] private int _eventNum = 0;

        [Header("Event Number 1")]
        [SerializeField] private WaitToggleSequence _waitToggleSequence;

        [Header("Event Num 2")]
        [SerializeField] private OpenMonitorSequence _monitorSequence;

        [Header("Event Number 3")]
        [SerializeField] private OpenHangerDoorSequence _openFirstDoor;

        [Header("Event Num 4")]
        [SerializeField] private MoveTargetSequence _moveSecondDoorTarget;

        [Header("Event Num 5")]
        [SerializeField] private WaitToggleSequence _secondDoorToggle;
        [SerializeField] private OpenHangerDoorSequence _openSecondDoor;
        [SerializeField] private MoveTargetSequence _moveOutside;
        
        
        //private SequenceData _data;
        
        // public void SetParams(float totalSec, int eventNum)
        // {
        //     _totalSec = totalSec;
        //     _eventNum = eventNum;
        // }

        public void SetData(SequenceData data)
        {
            //_data = data;
            _waitToggleSequence.SetData(data);
            _monitorSequence.SetData(data);
            _openFirstDoor.SetData(data);
            _moveSecondDoorTarget.SetData(data);
            _openSecondDoor.SetData(data);
            _moveOutside.SetData(data);
        }
        
        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            switch (_eventNum)
            {
                case 1:
                    await _waitToggleSequence.PlayAsync(ct, exceptionHandler);
                    break;
                case 2:
                    _monitorSequence.PlayAsync(ct, exceptionHandler).Forget(exceptionHandler);
                    break;
                case 3:
                    _openFirstDoor.PlayAsync(ct, exceptionHandler).Forget(exceptionHandler);
                    break;
                case 4:
                    _moveSecondDoorTarget.PlayAsync(ct, exceptionHandler).Forget(exceptionHandler);
                    break;
                case 5:
                    await _secondDoorToggle.PlayAsync(ct, exceptionHandler);
                    Event5Async(ct).Forget(exceptionHandler);
                    break;
            }

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        private async UniTask Event5Async(CancellationToken ct)
        {
            await _openSecondDoor.PlayAsync(ct);
            await _moveOutside.PlayAsync(ct);
        }

        public void Skip()
        {
            switch (_eventNum)
            {
                case 1:
                    _waitToggleSequence.Skip();
                    break;
            }
        }
    }
}
