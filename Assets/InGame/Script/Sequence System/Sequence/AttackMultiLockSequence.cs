using System.Threading;
using Cysharp.Threading.Tasks;

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
