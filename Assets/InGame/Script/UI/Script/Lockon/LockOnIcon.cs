using System;
using System.Collections.Generic;
using IronRain.Player;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;

public class LockOnIcon : MonoBehaviour
{
    [SerializeField] private LockOn _lockOn; // ロックオン機能
    [SerializeField] private LockOnIconView _iconPrefab; // ロックオンアイコンのプレハブ
    
    private IObjectPool<LockOnIconView> _iconPool; //ロックオンアイコンのオブジェクトプール
    private List<LockOnIconView> _activeIcons = new List<LockOnIconView>(); // アクティブなアイコンリスト
    private ReactiveProperty<Transform > _currentTarget = new ReactiveProperty<Transform>(); // 現在ロックオン中のターゲット

    private void Start()
    {
        InitializePool();
        SubscribeToTargetChanges();
    }

    /// <summary>
    /// オブジェクトプールの初期化を行う
    /// </summary>
    private void InitializePool()
    {
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
    }
    
    /// <summary>
    /// ターゲット変更を監視・変更時の処理の購読を行う
    /// </summary>
    private void SubscribeToTargetChanges()
    {
        _lockOn
            .ObserveEveryValueChanged(lockOn => lockOn.GetRockEnemy)
            .Subscribe(target => _currentTarget.Value = target?.transform)
            .AddTo(this);

        //ターゲットが変更されたときにアイコンの更新処理を呼び出す
        _currentTarget
            .Subscribe(_ => UpdateLockOnIcons())
            .AddTo(this);
    }

    /// <summary>
    /// ロックオンアイコンを更新する
    /// </summary>
    private void UpdateLockOnIcons()
    {
        // ロックオン中の敵がいない場合、アイコンを全て非表示にする
        if (_lockOn.GetRockEnemy == null)
        {
            ReleaseLockOnIconIcons();
            return;
        }
        
        ReleaseLockOnIconIcons();

        // プールから取り出す
        LockOnIconView icon = _iconPool.Get();
        _activeIcons.Add(icon);

        // アイコンの座標をセットする
        icon.transform.SetParent(_lockOn.GetRockEnemy.transform);
        icon.transform.localPosition = Vector3.zero;
        icon.transform.localScale = Vector3.one;

        icon.Initialize();

        // 敵死亡時のイベントを購読
        icon.OnRelease += ReleaseLockOnIcons;

    }

    /// <summary>
    /// 敵死亡時にアイコンをプールに戻す処理
    /// </summary>
    private void ReleaseLockOnIcons(LockOnIconView icon)
    {
        _iconPool.Release(icon);
        _activeIcons.Remove(icon);
        icon.OnRelease -= ReleaseLockOnIcons; // 敵死亡時のイベント購読解除
    }

    /// <summary>
    /// 表示中の全てのアイコンをプールに戻す処理
    /// </summary>
    private void ReleaseLockOnIconIcons()
    {
        foreach (var icon in _activeIcons)
        {
            _iconPool.Release(icon);
            icon.OnRelease -= ReleaseLockOnIcons; // 敵死亡時のイベント購読解除
        }
        _activeIcons.Clear();
    }
}
