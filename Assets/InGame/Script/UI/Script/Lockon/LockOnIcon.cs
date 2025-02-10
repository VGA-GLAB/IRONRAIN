using System.Collections.Generic;
using IronRain.Player;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;

public class LockOnIcon : MonoBehaviour
{
    [SerializeField] private LockOn _lockOn; // ロックオン機能
    [SerializeField] private GameObject _playerWeponContoller;
    [SerializeField] private LockOnIconView _iconPrefab; // ロックオンアイコンのプレハブ

    private PlayerWeaponController _playerWeapon;
    private IObjectPool<LockOnIconView> _iconPool; //ロックオンアイコンのオブジェクトプール
    private List<LockOnIconView> _activeIcons = new List<LockOnIconView>(); // アクティブなアイコンリスト
    private LockOnType _currentLockOnType = LockOnType.LockOn; // 現在のロックオンモード
    private ReactiveProperty<Transform > _currentTarget = new ReactiveProperty<Transform>(); // 現在ロックオン中のターゲット

    private void Start()
    {
        _playerWeapon = _playerWeponContoller.GetComponent<PlayerWeaponController>();
        _playerWeapon.WeaponModel.OnWeaponChange += HandleWeaponChange; // 武器切り替え時のイベントを購読

        // オブジェクトプールの初期化
        _iconPool = new ObjectPool<LockOnIconView>(
            createFunc: () => Instantiate(_iconPrefab, transform), //子オブジェクトに追加
            actionOnGet: icon => icon.gameObject.SetActive(true),
            actionOnRelease: icon =>
            {
                icon.gameObject.SetActive(false);
                icon.transform.SetParent(transform); // 子オブジェクトに戻す
            },
            actionOnDestroy: icon => Destroy(icon.gameObject),
            collectionCheck: false, 
            defaultCapacity: 3, // 初期サイズ
            maxSize: 10 // 最大サイズ
        );
        
        Observable
            .EveryUpdate()
            .Subscribe(_ =>
            {
                if (_lockOn.GetRockEnemy != null)
                {
                    _currentTarget.Value = _lockOn.GetRockEnemy.transform;
                }
            })
            .AddTo(this);
        
        //ターゲットが変更されたときにアイコンの更新処理を呼び出す
        _currentTarget.Subscribe(_ => UpdateLockOnIcons()).AddTo(this); 
    }

    private void OnDestroy()
    {
        _playerWeapon.WeaponModel.OnWeaponChange -= HandleWeaponChange; // 購読解除
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
        
        UpdateLockOnIcons(); //アイコン表示を更新する
    }
    
    /// <summary>
    /// ロックオンアイコンを更新する
    /// </summary>
    private void UpdateLockOnIcons()
    {
        if (_lockOn.GetRockEnemy != null)
        {
            Transform target = _lockOn.GetRockEnemy.transform;
            
            // ロックオンモードが正面限定の場合、アイコンを1つに保つためリセット処理を行う
            if (_currentLockOnType == LockOnType.FrontOnly)
            {
                ReleaseLockOnIcons();
            }
            
            // 既にターゲットに対応するアイコンがある場合、以降の処理は行わない
            if (_activeIcons.Exists(icon => icon.transform.parent == target))
            {
                Debug.Log($"ターゲット {target.name} には既にロックオンアイコンが表示されています");
                return;
            }
            
            // プールから取り出す
            LockOnIconView icon = _iconPool.Get();
            _activeIcons.Add(icon);
            
            // アイコンの座標をセットする
            icon.transform.SetParent(target);
            icon.transform.localPosition = Vector3.zero;
            icon.transform.localScale = Vector3.one;
            
            icon.Initialize();
            
            // 敵死亡時のイベントを購読
            icon.OnEnemyBroken += ReleaseLockOnIcons;
        }
    }

    /// <summary>
    /// 敵死亡時にアイコンをプールに戻す処理
    /// </summary>
    private void ReleaseLockOnIcons(LockOnIconView icon)
    {
        Debug.Log("死亡時のイベント");
        _iconPool.Release(icon);
        _activeIcons.Remove(icon);
        icon.OnEnemyBroken -= ReleaseLockOnIcons; // 敵死亡時のイベント購読解除
    }

    /// <summary>
    /// 表示中の全てのアイコンをプールに戻す処理
    /// </summary>
    private void ReleaseLockOnIcons()
    {
        foreach (var icon in _activeIcons)
        {
            _iconPool.Release(icon);
            icon.OnEnemyBroken -= ReleaseLockOnIcons; // 敵死亡時のイベント購読解除
        }
        _activeIcons.Clear();
    }
}
