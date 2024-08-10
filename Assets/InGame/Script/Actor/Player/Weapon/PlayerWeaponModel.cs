using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

namespace IronRain.Player
{
    public class PlayerWeaponModel : IPlayerStateModel
    {
        public PlayerWeaponBase CurrentWeapon => _playerWeaponList[_currentWeaponIndex];

        [SerializeField] private List<PlayerWeaponBase> _playerWeaponList = new();

        private bool _isWeaponChenge;
        private bool _isShot;
        //0始まり
        private int _currentWeaponIndex;
        private PlayerEnvroment _playerEnvroment;
        private CancellationToken _rootCancellOnDestroy;

        public void SetUp(PlayerEnvroment env, CancellationToken token)
        {
            _playerEnvroment = env;
            _rootCancellOnDestroy = token;
        }

        public void Start()
        {
            InputProvider.Instance.SetEnterInputAsync(InputProvider.InputType.WeaponChenge, WeaponChenge);
            for (int i = 0; i < _playerWeaponList.Count; i++)
            {
                _playerWeaponList[i].SetUp(_playerEnvroment);
            }
        }

        public void FixedUpdate()
        {

        }

        public void Update()
        {
            Shot();
        }

        /// <summary>
        /// 武器切り替え
        /// </summary>
        private async UniTaskVoid WeaponChenge()
        {
            if (_playerWeaponList[_currentWeaponIndex].WeaponParam.WeaponType
                == PlayerWeaponType.AssaultRifle)
            {
                await _playerEnvroment.PlayerAnimation.PlayerAssaultDusarm();
            }
            else
            {
                await _playerEnvroment.PlayerAnimation.PlayerRocketDisarm();
            }


            if (_currentWeaponIndex + 1 < _playerWeaponList.Count)
            {
                _currentWeaponIndex++;
            }
            else
            {
                _currentWeaponIndex = 0;
            }
            CriAudioManager.Instance.SE.Play("SE", "Change");
        }

        private void Shot()
        {
            _isShot = InputProvider.Instance.GetStayInput(InputProvider.InputType.OneButton);
            if (_playerEnvroment.PlayerState.HasFlag(PlayerStateType.SwitchingArms)
                || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.RepairMode)
                || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE)
                || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.NonAttack)
                || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.Inoperable)
                || !_isShot) return;

            _playerWeaponList[_currentWeaponIndex].Shot();
        }

        public void MulchShot()
        {
            _playerWeaponList[_currentWeaponIndex].MulchShot();
        }


        public void Dispose()
        {

        }
    }
}
