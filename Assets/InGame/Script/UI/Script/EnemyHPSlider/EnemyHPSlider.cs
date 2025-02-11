using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UniRx;
using UnityEngine;

public class EnemyHPSlider : MonoBehaviour
{
    [SerializeField] private LockOn _lockOn; // ロックオン機能
    [SerializeField] private EnemyHPSliderView _view; // スライダーの表示を変更するクラス
    
    private ReactiveProperty<Transform > _currentTarget = new ReactiveProperty<Transform>(); // 現在ロックオン中のターゲット
    

    private void Start()
    {
        SubscribeToTargetChanges();
    }
    
    /// <summary>
    /// ターゲット変更を監視・変更時の処理の購読を行う
    /// </summary>
    private void SubscribeToTargetChanges()
    {
        Observable
            .EveryUpdate()
            .Where(_ => _lockOn.GetRockEnemy != null)
            .Subscribe(_ => _currentTarget.Value = _lockOn.GetRockEnemy.transform)
            .AddTo(this);

        //ターゲットが変更されたときにアイコンの更新処理を呼び出す
        _currentTarget.Subscribe(_ => UpdateLockOnIcons()).AddTo(this);
    }

    /// <summary>
    /// スライダーを表示する対象を変更する
    /// </summary>
    private void UpdateLockOnIcons()
    {
        if(_lockOn.GetRockEnemy == null) return;
        
        BlackBoard enemyBB = _lockOn.GetRockEnemy.GetComponent<EnemyController>().BlackBoard as BlackBoard; // 現在のHPを取得する
        EnemyParams enemyParam = _lockOn.GetRockEnemy.GetComponent<EnemyParams>(); //最大HPを取得する
        
        // スライダーの操作を行う
        _view.transform.SetParent(_lockOn.GetRockEnemy.transform);
        _view.transform.localPosition = Vector3.zero;
        _view.transform.localScale = Vector3.one;
        _view.Initialize(enemyBB, enemyParam.MaxHp); //スライダーの値をセットする
    }
}
