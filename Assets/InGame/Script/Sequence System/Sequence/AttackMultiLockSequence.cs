using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using IronRain.Player;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    [Serializable]
    public sealed class AttackMultiLockSequence : ISequence
    {
        [OpenScriptButton(typeof(AttackMultiLockSequence))]
        [Description("マルチロックオンの攻撃処理を行うシーケンス")]
        private PlayerController _playerController;
        
        public void SetData(SequenceData data)
        {
            _playerController = data.PlayerController;
        }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // マルチロックオンの攻撃呼び出し
            _playerController.SeachState<PlayerWeaponController>().WeaponModel.MulchShot();
            
            return UniTask.CompletedTask;
        }

        public void Skip()
        {
        }
    }
}
