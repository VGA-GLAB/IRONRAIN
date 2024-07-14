using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    [Serializable]
    public class MoveTargetSequence : ISequence
    {
        public enum MoveTarget
        {
            SecondDoor,
            Outside
        }
        
        [SerializeField] private float _totalSec = 0F;
        [SerializeField] private float _moveSec = 0F;
        [SerializeField] private MoveTarget _target = MoveTarget.SecondDoor;

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
            Move(ct).Forget();

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        private async UniTask Move(CancellationToken ct)
        {
            if (_target == MoveTarget.SecondDoor)
            {
                await _data.PlayerTransform.DOMove(_data.SecondHatchTarget.position, _moveSec)
                    .ToUniTask(cancellationToken: ct);
            }
            else
            {
                await _data.PlayerTransform.DOMove(_data.HangerOutsideTarget.position, _moveSec)
                    .ToUniTask(cancellationToken: ct);
            }
        }

        public void Skip()
        {
            if (_target == MoveTarget.SecondDoor)
            {
                _data.PlayerTransform.position = _data.SecondHatchTarget.position;
            }
            else
            {
                _data.PlayerTransform.position = _data.SecondHatchTarget.position;
            }
        }
    }
}
