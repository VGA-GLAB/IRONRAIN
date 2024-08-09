using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class RemovePlayerStateSequence : ISequence
    {
        [OpenScriptButton(typeof(RemovePlayerStateSequence))]
        [Description("プレイヤーの状態を解除するシーケンス")]
        [Header("このSequenceを抜けるまでの時間(秒)"), SerializeField] private float _totalSec = 0F;
        [Header("解除するPlayerのState"), SerializeField] private PlayerStateType _removeState;

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

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
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
