using System.Collections.Generic;
using IronRain.Player;
using UnityEngine;
using UnityEngine.Pool;

public class LockOnIcon : MonoBehaviour
{
    [SerializeField] private LockOn _lockOn; // ロックオン機能
    [SerializeField] private GameObject _playerWeponContoller;
    [SerializeField] private LockOnIconView _iconPrefab; // ロックオンアイコンのプレハブ
    [SerializeField] private Canvas _canvas; // ワールドスペースCanvas

    private PlayerWeaponController _playerWeapon;
    private IObjectPool<LockOnIconView> _iconPool; //ロックオンアイコンのオブジェクトプール
    private List<LockOnIconView> _activeIcons = new List<LockOnIconView>(); // アクティブなアイコンリスト
    private LockOnType _currentLockOnType; // 現在のロックオンモード

    private void Start()
    {
        _playerWeapon = _playerWeponContoller.GetComponent<PlayerWeaponController>();
        
        _playerWeapon.WeaponModel.OnWeaponChange += HandleWeaponChange; // 武器切り替え時のイベントを購読

        Debug.Log("呼ばれた");
        // オブジェクトプールの初期化
        _iconPool = new ObjectPool<LockOnIconView>(
            createFunc: () => Instantiate(_iconPrefab, _canvas.transform),
            actionOnGet: icon => icon.gameObject.SetActive(true),
            actionOnRelease: icon => icon.gameObject.SetActive(false),
            actionOnDestroy: icon => Destroy(icon.gameObject),
            collectionCheck: false, 
            defaultCapacity: 3, // 初期サイズ
            maxSize: 10 // 最大サイズ
        );
    }

    private void OnDestroy()
    {
        _playerWeapon.WeaponModel.OnWeaponChange -= HandleWeaponChange; // 購読解除
    }

    private void Update()
    {
        UpdateLockOnIcons();
    }

    /// <summary>
    /// 武器が変更されたときロックオンモードを変更する
    /// </summary>
    private void HandleWeaponChange()
    {
        switch (_playerWeapon.WeaponModel.CurrentWeapon.WeaponParam.WeaponType)
        {
            case PlayerWeaponType.AssaultRifle:
                _currentLockOnType = LockOnType.FrontOnly; //アサルトライフルは正面限定
                break;
            case PlayerWeaponType.RocketLauncher:
                _currentLockOnType = LockOnType.LockOn; //バズーカはロックオン中の敵
                break;
            default:
                Debug.LogWarning($"登録されていない武器種です。ロックオンのモードが変更できません");
                break;
        }
    }
    
    /// <summary>
    /// ロックオンアイコンを更新する
    /// </summary>
    private void UpdateLockOnIcons()
    {
        // 以前のアイコンをプールに戻す
        foreach (var icon in _activeIcons)
        {
            _iconPool.Release(icon);
        }
        _activeIcons.Clear();

        // 現在のロックオン対象を取得

        if (_currentLockOnType == LockOnType.LockOn)
        {
            LockOnIconView icon = _iconPool.Get();
            icon.SetTarget(_lockOn.GetRockEnemy.transform);
            _activeIcons.Add(icon);
        }
        
        /*
        foreach (var target in _lockOn.GetLockOnTargets(_currentLockOnType))
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
            if (screenPos.z > 0) //カメラの後ろにいる場合は無視
            {
                LockOnIconView icon = _iconPool.Get();
                icon.SetTarget(target);
                _activeIcons.Add(icon);
            }
        }
        */
    }
}
