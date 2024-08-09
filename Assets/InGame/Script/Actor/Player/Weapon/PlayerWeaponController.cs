using UnityEngine;

namespace IronRain.Player
{
    public class PlayerWeaponController : PlayerComponentBase
    {
        public PlayerWeaponModel WeaponModel { get; private set; }

        [SerializeField] private PlayerWeaponView _playerWeaponView; 

        private void Awake()
        {
            WeaponModel = _playerStateModel as PlayerWeaponModel;
            _playerStateView = _playerWeaponView;
        }
    }
}
