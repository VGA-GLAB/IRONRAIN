using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotInputDebug : MonoBehaviour
{
    [SerializeField] private PlayerController _robotController;
    [SerializeField] private GameObject _ui;
    [SerializeField] private Text _rightInput;
    [SerializeField] private Text _leftInput;
    [SerializeField] private Text _moveState;
    [SerializeField] private Text _currentWeapon;
    [SerializeField] private Text _currentBullet;
    [SerializeField] private Text _maxBullet;

    private PlayerMove _playerMove;
    private PlayerWeaponController _weaponCon;
    private bool _active = false;

    private void Start()
    {
        _playerMove = _robotController.SeachState<PlayerMove>();
        _weaponCon = _robotController.SeachState<PlayerWeaponController>();
    }

    private void Update()
    {
        SetView();
    }

    public void Active()
    {
        _active = true;
        _ui.SetActive(true);
    }

    private void SetView()
    {
        if (!_active) return;

        _leftInput.text = InputProvider.Instance.LeftLeverDir.z.ToString();
        _rightInput.text = InputProvider.Instance.RightLeverDir.z.ToString();
        _maxBullet.text = _weaponCon.CurrentWeapon.WeaponParam.MagazineSize.ToString();
        _currentBullet.text = _weaponCon.CurrentWeapon.CurrentBullets.ToString();
        _currentWeapon.text = _weaponCon.CurrentWeapon.ToString();
    }
}
