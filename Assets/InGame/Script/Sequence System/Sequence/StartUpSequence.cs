using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        [SerializeField] private OpenHangerDoorSequence _openHanger;
        
        private SequenceData _data;
        
        public void SetParams(float totalSec, int eventNum)
        {
            _totalSec = totalSec;
            _eventNum = eventNum;
        }

        public void SetData(SequenceData data)
        {
            _data = data;
        }
        
        public async UniTask PlayAsync(CancellationToken ct)
        {
            switch (_eventNum)
            {
                case 1:
                    _waitToggleSequence.PlayAsync(ct).Forget();
                    break;
                case 2:
                    _monitorSequence.PlayAsync(ct).Forget();
                    break;
                case 3:
                    _openHanger.PlayAsync(ct).Forget();
                    break;
                case 4:
                    
            }

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
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
