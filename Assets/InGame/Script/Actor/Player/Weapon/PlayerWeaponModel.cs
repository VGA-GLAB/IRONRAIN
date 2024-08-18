using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System;

namespace IronRain.Player
{
    public class PlayerWeaponModel : IPlayerStateModel
    {
        public event Action OnShot;
        public event Action OnWeaponChange;
        public PlayerWeaponBase CurrentWeapon => _playerWeaponList[_currentWeaponIndex];

        [SerializeField] private List<PlayerWeaponBase> _playerWeaponList = new();
        [SerializeField] private RootMotion.FinalIK.FABRIK _fabrIk;
        [SerializeField] private RaderMap _raderMap;
        [SerializeField] private float _aimSpeed;
        [SerializeField] private Transform _defaultAimTarget;

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
            InputProvider.Instance.SetEnterInputAsync(InputProvider.InputType.TwoButton, WeaponChenge);
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
            if (_raderMap.GetRockEnemy != null)
            {
                _fabrIk.solver.target = _raderMap.GetRockEnemy.transform;
            }
            else 
            {
                _fabrIk.solver.target = _defaultAimTarget;
            }
        }

        /// <summary>
        /// 武器切り替え
        /// </summary>
        private async UniTaskVoid WeaponChenge()
        {
            _playerWeaponList[_currentWeaponIndex].WeaponObject.SetActive(false);

            if (_playerWeaponList[_currentWeaponIndex].WeaponParam.WeaponType
                == PlayerWeaponType.AssaultRifle)
            {
                await _playerEnvroment.PlayerAnimation.PlayerAssaultDusarm();
            }
            else
            {
                await _playerEnvroment.PlayerAnimation.PlayerRocketDisarm();
            }

            OnWeaponChange?.Invoke();


            if (_currentWeaponIndex + 1 < _playerWeaponList.Count)
            {
                _currentWeaponIndex++;
            }
            else
            {
                _currentWeaponIndex = 0;
            }

            _playerWeaponList[_currentWeaponIndex].WeaponObject.SetActive(true);
            CriAudioManager.Instance.SE.Play("SE", "SE_Change");
        }

        /// <summary>
        /// 弾の発射
        /// </summary>
        private void Shot()
        {
            _isShot = InputProvider.Instance.GetStayInput(InputProvider.InputType.OneButton);
            if (_playerEnvroment.PlayerState.HasFlag(PlayerStateType.SwitchingArms)
                || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.RepairMode)
                || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE)
                || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.NonAttack)
                || _playerEnvroment.PlayerState.HasFlag(PlayerStateType.Inoperable)
                || !_isShot) return;

            OnShot?.Invoke();
            _playerWeaponList[_currentWeaponIndex].Shot();
        }

        public void MulchShot()
        {
            _playerWeaponList[_currentWeaponIndex].MulchShot();
        }

        public PlayerWeaponBase GetWepaon(PlayerWeaponType playerWeaponType)
        {
            for (int i = 0; i < _playerWeaponList.Count; i++) 
            {
                if (_playerWeaponList[i].WeaponParam.WeaponType == playerWeaponType) 
                {
                    return _playerWeaponList[i];
                }
            }

            return null;
        }


        public void Dispose()
        {
            OnShot = null;
            OnWeaponChange = null;
        }
    }
}
