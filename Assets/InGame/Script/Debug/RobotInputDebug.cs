using IronRain.SequenceSystem;
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
    [SerializeField] private Text _currentSeq;
    [SerializeField] private SequencePlayer _sequencePlayer;

    private PlayerBossMove _playerMove;
    private PlayerWeaponController _weaponCon;
    private PlayerQTE _playerQTE;
    private bool _active = false;

    public void SetUp() 
    {

    }

    private void Start()
    {
        //_playerMove = _robotController.SeachState<PlayerMove>();
        _weaponCon = _robotController.SeachState<PlayerWeaponController>();
        _playerQTE = _robotController.SeachState<PlayerQTE>();
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

        _leftInput.text = $"X:{InputProvider.Instance.LeftLeverDir.x}Z:{InputProvider.Instance.LeftLeverDir.z}";
        _rightInput.text = $"X:{InputProvider.Instance.RightLeverDir.x}Z:{InputProvider.Instance.RightLeverDir.z}";
        _maxBullet.text = _weaponCon.WeaponModel.CurrentWeapon.WeaponParam.MagazineSize.ToString();
        _currentBullet.text = _weaponCon.WeaponModel.CurrentWeapon.CurrentBullets.ToString();
        _currentWeapon.text = _weaponCon.WeaponModel.CurrentWeapon.ToString().Replace("PlayerWeapon (", "");
        _moveState.text = _playerQTE.QTEModel.QTEType.Value.ToString();
        _currentSeq.text = _sequencePlayer.CurrentSequence == null 
            ? "<null>" 
            : _sequencePlayer.CurrentSequence.ToString().Replace("IronRain.SequenceSystem", "");
    }
}
