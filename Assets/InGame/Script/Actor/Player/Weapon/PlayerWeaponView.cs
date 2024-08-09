using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace IronRain.Player
{
    public class PlayerWeaponView : IPlayerStateView
    {
        [SerializeField] private GameObject _assaultRifle;
        [SerializeField] private GameObject _rocketLauncher;

        public void SetUp(PlayerEnvroment env, CancellationToken token)
        {
            
        }

        public void WeaponChange() 
        {
            _assaultRifle.SetActive(!_assaultRifle.activeSelf);
            _rocketLauncher.SetActive(!_rocketLauncher.activeSelf);
        }

    }
}
