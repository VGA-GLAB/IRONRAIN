using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BulletPool))]
public abstract class PlayerWeaponBase : MonoBehaviour
{
    public PlayerWeaponParams WeaponParam => _params;
    public int CurrentBullets => _currentBullets;

    [SerializeField] protected PlayerWeaponParams _params;
    [SerializeField] protected string _shotSeCueName;
    [SerializeField] protected BulletPool _bulletPool;

    protected bool _isShotInput;
    [Tooltip("現在の弾数")]
    protected int _currentBullets;
    private float _currentTime;
    [Tooltip("射撃中かどうか")]
    private bool _isFire;
    [Tooltip("リロード中かどうか")]
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
        if (_playerEnvroment == null) return;
        //_playerEnvroment.RaderMap.NearEnemyLockon();
        //次の発射までの計算
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
            //後でオブジェクトプールに
            var bulletCon = _bulletPool.GetBullet();
            bulletCon.SetUp(
                _playerEnvroment.RaderMap.GetRockEnemy, 
                _params.ShotDamage,
                _playerEnvroment.PlayerTransform.forward,
                _params.WeaponName);

            CriAudioManager.Instance.SE.Play("SE", _shotSeCueName);

            _isFire = false;
            _currentTime = 0;
            _currentBullets--;
        }

        if (_currentBullets == 0)
        {
            Reload();
        }
    }


    /// <summary>
    /// マルチショット
    /// </summary>
    public void MulchShot()
    {
        var lockOnEnemys = _playerEnvroment.RaderMap.MultiLockEnemys;

        for (int i = 0; i < lockOnEnemys.Count; i++)
        {
            var bulletCon = _bulletPool.GetBullet();
            bulletCon.SetUp(
                _playerEnvroment.RaderMap.MultiLockEnemys[i],
                _params.ShotDamage,
                _playerEnvroment.PlayerTransform.forward,
                _params.WeaponName);
        }
    }

    /// <summary>
    /// リロードの処理
    /// </summary>
    protected virtual void Reload()
    {
        _isReload = true;
        //アニメーション挟む
        _currentBullets = _params.MagazineSize;
        _isReload = false;
    }

}
