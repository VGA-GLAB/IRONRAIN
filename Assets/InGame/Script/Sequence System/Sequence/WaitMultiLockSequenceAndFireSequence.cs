using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class WaitMultiLockSequenceAndFireSequence : ISequence
    {
        private MouseMultilockSystem _mouseMultiLockSystem;
        private PlayerController _playerController;
        
        public void SetData(SequenceData data)
        {
            _mouseMultiLockSystem = data.MouseMultiLockSystem;
            _playerController = data.PlayerController;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            await UniTask.WaitUntil(() => !_mouseMultiLockSystem.IsMultilock, cancellationToken: ct);
            _playerController.SeachState<PlayerWeaponController>().WeaponModel.MulchShot();
        }

        public void Skip() { }
    }
}
