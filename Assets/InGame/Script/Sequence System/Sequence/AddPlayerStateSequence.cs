using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class AddPlayerStateSequence : ISequence
    {
        [SerializeField] private float _totalSec = 0F;
        [SerializeField] private PlayerStateType _addState;

        private PlayerController _playerController;

        public void SetParams(float totalSec, PlayerStateType addState)
        {
            _totalSec = totalSec;
            _addState = addState;
        }

        public void SetData(SequenceData data)
        {
            _playerController = data.PlayerController;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            _playerController.PlayerEnvroment.AddState(_addState);

            await UniTask.WaitForSeconds(_totalSec, cancellationToken: ct);
        }

        public void Skip()
        {
            _playerController.PlayerEnvroment.AddState(_addState);
        }
    }
}
