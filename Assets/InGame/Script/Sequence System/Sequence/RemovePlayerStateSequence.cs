using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class RemovePlayerStateSequence : ISequence
    {
        [SerializeField] private float _totalSec = 0F;
        [SerializeField] private PlayerStateType _removeState;

        private PlayerController _playerController;

        public void SetParams(float totalSec, PlayerStateType removeState)
        {
            _totalSec = totalSec;
            _removeState = removeState;
        }
        
        public void SetData(SequenceData data)
        {
            _playerController = data.PlayerController;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            _playerController.PlayerEnvroment.RemoveState(_removeState);

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _playerController.PlayerEnvroment.RemoveState(_removeState);
        }
    }
}
