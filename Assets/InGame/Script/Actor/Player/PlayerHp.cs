using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using IronRain.ShaderSystem;

namespace IronRain.Player
{
    public class PlayerHp : MonoBehaviour, IDamageable
    {
        public event Action OnDownEvent;
        [SerializeField] Camera _mainCamera;
        [SerializeField] private float _time;
        [SerializeField] private float _strength;
        [SerializeField] private HpManager _hpManager;
        [SerializeField] private WindowMaterialController _materialController;

        private int _hp;
        private int _maxHp;
        private PlayerEnvroment _playerEnvroment;

        public void Setup(PlayerEnvroment playerEnvroment)
        {
            _playerEnvroment = playerEnvroment;
            _hp = _playerEnvroment.PlayerSetting.PlayerParamsData.Hp;
            _maxHp = _hp;
        }

        public void Damage(int value, string weapon = "")
        {
            _hp = Mathf.Max(_hp - value, 0);
            _mainCamera.DOShakePosition(_time, _strength);

            if (weapon == PlayerWeaponType.AssaultRifle.ToString())
            {
                CriAudioManager.Instance.SE.Play("SE", "SE_Damage_02");
            }
            else if (weapon == PlayerWeaponType.RocketLauncher.ToString()) 
            {
                CriAudioManager.Instance.SE.Play("SE", "SE_Damage_01");
            }

            WindowCrack();
            
            _hpManager.BodyDamage(value);
            if (_hp == 0)
            {
                OnDownEvent?.Invoke();
            }
        }

        private void WindowCrack()
        {
            var current = (float)_hp / _maxHp;

            if (current < 0.8F)
            {
                _materialController.Crack(WindowMaterialController.CrackType.Type1);
            }
            else if (current < 0.6F)
            {
                _materialController.Crack(WindowMaterialController.CrackType.Type2);
            }
            else if (current < 0.4F)
            {
                _materialController.Crack(WindowMaterialController.CrackType.Type3);
            }
            else if (current < 0.2F)
            {
                _materialController.Crack(WindowMaterialController.CrackType.Type4);
            }
            else
            {
                _materialController.Crack(WindowMaterialController.CrackType.None);
            }
        }
    }
}
