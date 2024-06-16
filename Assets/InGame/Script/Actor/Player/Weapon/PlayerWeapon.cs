using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerWeaponBase : MonoBehaviour
{
    public PlayerWeaponParams WeaponParam => _params;
    public int CurrentBullets => _currentBullets;

    [Header("弾のPrefab")]
    [SerializeField] protected GameObject _bulletPrefab;
    [SerializeField] protected Transform _bulletInsPos;
    [SerializeField] protected PlayerWeaponParams _params;

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
        _playerEnvroment.RaderMap.NearEnemyLockon();
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
            var obj = Instantiate(_bulletPrefab, _bulletInsPos.position, Quaternion.identity);
            obj.GetComponent<BulletCon>().SetUp(_playerEnvroment.RaderMap.GetRockEnemy, _params.ShotDamage);
            _isFire = false;
            _currentTime = 0;
            _currentBullets--;
            Debug.Log("弾を打った");
        }

        if (_currentBullets == 0)
        {
            Reload();
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
