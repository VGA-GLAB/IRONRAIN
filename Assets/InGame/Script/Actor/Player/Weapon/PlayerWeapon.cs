using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerWeaponBase : MonoBehaviour
{
    public PlayerWeaponParams WeaponParam => _params;
    public int CurrentBullets => _currentBullets;

    [Header("�e��Prefab")]
    [SerializeField] protected GameObject _bulletPrefab;
    [SerializeField] protected Transform _bulletInsPos;
    [SerializeField] protected PlayerWeaponParams _params;

    protected bool _isShotInput;
    [Tooltip("���݂̒e��")]
    protected int _currentBullets;
    private float _currentTime;
    [Tooltip("�ˌ������ǂ���")]
    private bool _isFire;
    [Tooltip("�����[�h�����ǂ���")]
    private bool _isReload;
    private PlayerEnvroment _playerEnvroment;

    public virtual void SetUp(PlayerEnvroment playerEnvroment) 
    {
        _playerEnvroment = playerEnvroment;
    }

    protected virtual void Start()
    {
        _currentBullets = _params.MagazineSize;
    }

    private void Update()
    {
        //���̔��˂܂ł̌v�Z
        _currentTime += Time.deltaTime;
        if (_currentTime > _params.ShotRate)
        {
            _isFire = true;
        }
    }

    public virtual void Shot()
    {
        if (_isFire && 0 < _currentBullets && !_isReload)
        {
            //��ŃI�u�W�F�N�g�v�[����
            var obj = Instantiate(_bulletPrefab, _bulletInsPos);
            obj.GetComponent<BulletCon>().SetUp(_playerEnvroment.RaderMap.NearEnemy().obj, _params.ShotDamage);
            _isFire = false;
            _currentTime = 0;
            _currentBullets--;
            Debug.Log("�e��ł���");
        }

        if (_currentBullets == 0)
        {
            Reload();
        }
    }

    /// <summary>
    /// �����[�h�̏���
    /// </summary>
    protected virtual void Reload()
    {
        _isReload = true;
        //�A�j���[�V��������
        _currentBullets = _params.MagazineSize;
        _isReload = false;
    }

}
