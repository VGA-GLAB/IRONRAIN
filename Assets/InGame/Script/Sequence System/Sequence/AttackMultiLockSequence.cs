using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public sealed class AttackMultiLockSequence : ISequence
    {
        private PlayerController _playerController;
        
        public void SetData(SequenceData data)
        {
            _playerController = data.PlayerController;
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            _playerController.SeachState<PlayerWeaponController>().WeaponModel.MulchShot();
        }

        public void Skip()
        {
        }
    }
}
