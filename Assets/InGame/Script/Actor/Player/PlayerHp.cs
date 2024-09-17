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
        private Tween _shakeTween;
        private PlayerSetting.PlayerParams _param;

        public void Setup(PlayerEnvroment playerEnvroment)
        {
            _playerEnvroment = playerEnvroment;
            _param = playerEnvroment.PlayerSetting.PlayerParamsData;
            _hp = _playerEnvroment.PlayerSetting.PlayerParamsData.Hp;
            _maxHp = _hp;
        }

        public void Damage(int value, string weapon = "")
        {
            _hp = Mathf.Max(_hp - value, 0);

            if (weapon == PlayerWeaponType.AssaultRifle.ToString())
            {
                CriAudioManager.Instance.SE.Play3D(_playerEnvroment.PlayerTransform.position, "SE", "SE_Damage_02");
            }
            else if (weapon == PlayerWeaponType.RocketLauncher.ToString())
            {
                CriAudioManager.Instance.SE.Play3D(_playerEnvroment.PlayerTransform.position, "SE", "SE_Damage_01");
                if (_shakeTween == null)
                {
                    _shakeTween = _mainCamera.DOShakePosition(_time, _strength)
                        .OnComplete(() => { _shakeTween = null; })
                        .SetLink(gameObject);
                }
            }
            else 
            {
                if (_shakeTween == null)
                {
                    _shakeTween = _mainCamera.DOShakePosition(_time, _strength)
                        .OnComplete(() => { _shakeTween = null; })
                        .SetLink(gameObject);
                }
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

            if (current < _param.Crack1)
            {
                _materialController.Crack(WindowMaterialController.CrackType.Type1);
            }
            else if (current < _param.Crack2)
            {
                _materialController.Crack(WindowMaterialController.CrackType.Type2);
            }
            else if (current < _param.Crack3)
            {
                _materialController.Crack(WindowMaterialController.CrackType.Type3);
            }
            else if (current < _param.Crack4)
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
