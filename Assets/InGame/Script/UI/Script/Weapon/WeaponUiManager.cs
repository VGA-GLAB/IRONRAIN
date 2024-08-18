﻿using DG.Tweening;
using IronRain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponUiState
{
    Assault,
    RocketLauncher
}
public class WeaponUiManager : MonoBehaviour
{
    [Header("プレイヤーウェポンモデル")]
    [SerializeField] private PlayerWeaponController _weaponController;
    [Header("Uiの状態")]
    public WeaponUiState WeaponUiState;
    //[Header("アニメーションの時間（発射インターバルからどれだけ早くするか）")]
    //[SerializeField] private float _textInterval;

    [Header("アサルトライフル")]
    [Header("アサルトライフルのUiオブジェクト")]
    [SerializeField] private GameObject _assultUiGameObject;
    [Header("残弾の最大値テキスト")]
    [SerializeField] private Text _assultBulletMaxText;
    [Header("残弾数テキスト")]
    [SerializeField] private Text _assultBulletCurrentText;
    [Header("武器残弾ゲージ")]
    [SerializeField] private Image _assultBulletWeaponGauge;
    [Header("円残弾ゲージ")]
    [SerializeField] private Image _assultBulletCircleGauge;
    [Header("リロード中フラグ")]
    public bool IsAssultReload = false;
    [Header("リロード時間")]
    private float _assultReloadTime = 5.0f;
    [Header("残弾の最大値")]
    private int _assultBulletMaxCount = 20;
    //[Header("発射のインターバル")]
    //private float _assultInterval = 0.1f;
    private float _assultCurrentCount;
    


    private PlayerWeaponBase _assultBase;

    [Header("ロケットランチャー")]
    [Header("ロケットランチャーのUiオブジェクト")]
    [SerializeField] private GameObject _rocketLauncherUiGameObject;
    [Header("残弾の最大値テキスト")]
    [SerializeField] private Text _rocketLauncherBulletMaxText;
    [Header("残弾数テキスト")]
    [SerializeField] private Text _rocketLauncherBulletCurrentText;
    [Header("武器残弾ゲージ")]
    [SerializeField] private Image _rocketLauncherBulletWeaponGauge;
    [Header("円残弾ゲージ")]
    [SerializeField] private Image _rocketLauncherBulletCircleGauge;
    [Header("リロード中フラグ")]
    public bool IsRocketLauncherReload = false;
    [Header("リロード時間")]
    private float _rocketLauncherReloadTime = 5.0f;
    [Header("残弾の最大値")]
    private int _rocketLauncherBulletMaxCount = 5;
    //[Header("発射のインターバル")]
    //private float _rocketInterval = 0.1f;
    private float _rocketLauncherCurrentCount;

    private PlayerWeaponBase _rocketBase;


    private void Awake()
    {
        //データを取ってくる
        if(_weaponController != null)
        {
            _assultBase = _weaponController.WeaponModel.GetWepaon(PlayerWeaponType.AssaultRifle);
            _rocketBase = _weaponController.WeaponModel.GetWepaon(PlayerWeaponType.RocketLauncher);
        }  
    }

    // Start is called before the first frame update
    void Start()
    {
        //初期化する
        if(_assultBase != null)
        {
            var assultParam = _assultBase.WeaponParam;
            _assultBulletMaxCount = assultParam.MagazineSize;
            _assultReloadTime = assultParam.ReloadTime;
            //_assultInterval = assultParam.ShotRate;
        }

        if(_rocketBase != null)
        {
            var rocketParam = _rocketBase.WeaponParam;
            _rocketLauncherBulletMaxCount = rocketParam.MagazineSize;
            _rocketLauncherReloadTime = rocketParam.ReloadTime;
            //_rocketInterval = rocketParam.ShotRate;
        }

        //現在の残弾とゲージを初期化
        _assultCurrentCount = _assultBulletMaxCount;
        _assultBulletMaxText.text = _assultBulletMaxCount.ToString();
        _assultBulletCurrentText.text = _assultCurrentCount.ToString();
        _assultBulletWeaponGauge.fillAmount = 1.0f;
        _assultBulletCircleGauge.fillAmount = 1.0f;

        _rocketLauncherCurrentCount = _rocketLauncherBulletMaxCount;
        _rocketLauncherBulletMaxText.text = _rocketLauncherBulletMaxCount.ToString();
        _rocketLauncherBulletCurrentText.text = _rocketLauncherCurrentCount.ToString();
        _rocketLauncherBulletWeaponGauge.fillAmount = 1.0f;
        _rocketLauncherBulletCircleGauge.fillAmount = 1.0f;
        
        ChangeWeapon(WeaponUiState);
    }

    /// <summary>
    /// 弾を打つときに呼ぶ処理
    /// </summary>
    public bool WeaponShot()
    {
        if (WeaponUiState == WeaponUiState.Assault)
        {
            if (IsAssultReload)
                return false;

            _assultCurrentCount--;
            //テキストを更新
            _assultBulletCurrentText.text = _assultCurrentCount.ToString();
            //ゲージを更新
            _assultBulletWeaponGauge.fillAmount = _assultCurrentCount / _assultBulletMaxCount;
            _assultBulletCircleGauge.fillAmount = _assultCurrentCount / _assultBulletMaxCount;

            if (_assultCurrentCount <= 0)
            {
                IsAssultReload = true;
                // DoTweenを使ってゲージのfillAmountをアニメーション
                Sequence sequence = DOTween.Sequence();
                sequence.Append(_assultBulletWeaponGauge.DOFillAmount(1, _assultReloadTime).SetEase(Ease.Linear))
                    .SetLink(this.gameObject);
                sequence.Join(_assultBulletCircleGauge.DOFillAmount(1, _assultReloadTime).SetEase(Ease.Linear)
                    .OnUpdate(() => UpdatePercentageText(WeaponUiState.Assault)).OnComplete(() =>
                    {
                        IsAssultReload = false;
                        _assultCurrentCount = _assultBulletMaxCount;
                    }).SetLink(this.gameObject)
                );

                sequence.SetAutoKill(true);

                // シーケンスを開始
                sequence.Play();
            }
        }
        else
        {
            if (IsRocketLauncherReload)
                return false;

            _rocketLauncherCurrentCount--;
            //テキストを更新
            _rocketLauncherBulletCurrentText.text = _rocketLauncherCurrentCount.ToString();
            //ゲージを更新
            _rocketLauncherBulletWeaponGauge.fillAmount =
                _rocketLauncherCurrentCount / _rocketLauncherBulletMaxCount;
            _rocketLauncherBulletCircleGauge.fillAmount =
                _rocketLauncherCurrentCount / _rocketLauncherBulletMaxCount;

            if (_rocketLauncherCurrentCount <= 0)
            {
                IsRocketLauncherReload = true;
                // DoTweenを使ってゲージのfillAmountをアニメーション
                Sequence sequence = DOTween.Sequence();
                sequence.Append(_rocketLauncherBulletWeaponGauge.DOFillAmount(1, _assultReloadTime)
                        .SetEase(Ease.Linear))
                    .SetLink(this.gameObject);
                sequence.Join(_rocketLauncherBulletCircleGauge.DOFillAmount(1, _assultReloadTime)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() => UpdatePercentageText(WeaponUiState.RocketLauncher)).OnComplete(() =>
                    {
                        IsRocketLauncherReload = false;
                        _rocketLauncherCurrentCount = _rocketLauncherBulletMaxCount;
                    }).SetLink(this.gameObject)
                );

                sequence.SetAutoKill(true);

                // シーケンスを開始
                sequence.Play();
            }
        }
        return true;
    }

    /// <summary>
    /// 武器切り替えを行う処理
    /// </summary>
    /// <param name="changeState"></param>
    public void ChangeWeapon(WeaponUiState changeState)
    {
        WeaponUiState = changeState;
        
        if(changeState == WeaponUiState.Assault)
        {
            _assultUiGameObject.transform.SetAsLastSibling();
        }
        else
        {
            _rocketLauncherUiGameObject.transform.SetAsLastSibling();
        }
    }

    /// <summary>
    /// ％テキストを更新する処理
    /// </summary>
    private void UpdatePercentageText(WeaponUiState weaponUi)
    {
        if(weaponUi == WeaponUiState.Assault)
        {
            float percentage = _assultBulletCircleGauge.fillAmount;
            int currentBullet = Mathf.FloorToInt(_assultBulletMaxCount * percentage);;
            _assultBulletCurrentText.text = currentBullet.ToString();
            
        }
        else
        {
            float percentage = _rocketLauncherBulletCircleGauge.fillAmount;
            int currentBullet = Mathf.FloorToInt(_rocketLauncherBulletMaxCount * percentage);
            _rocketLauncherBulletCurrentText.text = currentBullet.ToString();
        }
    }



    ////////////////テスト用////////////////////
    public void ChangeWeaponTest()
    {
        
        if(WeaponUiState == WeaponUiState.Assault)
        {
            _rocketLauncherUiGameObject.transform.SetAsLastSibling();
            WeaponUiState = WeaponUiState.RocketLauncher;
        }
        else
        {
            _assultUiGameObject.transform.SetAsLastSibling();
            WeaponUiState = WeaponUiState.Assault;
        }
    }

    public void WeaponShotTest()
    {
        if (WeaponUiState == WeaponUiState.Assault)
        {
            if (IsAssultReload)
                return;

            _assultCurrentCount--;
            //テキストを更新
            _assultBulletCurrentText.text = _assultCurrentCount.ToString();
            //ゲージを更新
            _assultBulletWeaponGauge.fillAmount = _assultCurrentCount / _assultBulletMaxCount;
            _assultBulletCircleGauge.fillAmount = _assultCurrentCount / _assultBulletMaxCount;

            if (_assultCurrentCount <= 0)
            {
                IsAssultReload = true;
                // DoTweenを使ってゲージのfillAmountをアニメーション
                Sequence sequence = DOTween.Sequence();
                sequence.Append(_assultBulletWeaponGauge.DOFillAmount(1, _assultReloadTime).SetEase(Ease.Linear))
                    .SetLink(this.gameObject);
                sequence.Join(_assultBulletCircleGauge.DOFillAmount(1, _assultReloadTime).SetEase(Ease.Linear)
                    .OnUpdate(() => UpdatePercentageText(WeaponUiState.Assault)).OnComplete(() =>
                    {
                        IsAssultReload = false;
                        _assultCurrentCount = _assultBulletMaxCount;
                    }).SetLink(this.gameObject)
                );

                sequence.SetAutoKill(true);

                // シーケンスを開始
                sequence.Play();
            }
        }
        else
        {
            if (IsRocketLauncherReload)
                return;

            _rocketLauncherCurrentCount--;
            //テキストを更新
            _rocketLauncherBulletCurrentText.text = _rocketLauncherCurrentCount.ToString();
            //ゲージを更新
            _rocketLauncherBulletWeaponGauge.fillAmount =
                _rocketLauncherCurrentCount / _rocketLauncherBulletMaxCount;
            _rocketLauncherBulletCircleGauge.fillAmount =
                _rocketLauncherCurrentCount / _rocketLauncherBulletMaxCount;

            if (_rocketLauncherCurrentCount <= 0)
            {
                IsRocketLauncherReload = true;
                // DoTweenを使ってゲージのfillAmountをアニメーション
                Sequence sequence = DOTween.Sequence();
                sequence.Append(_rocketLauncherBulletWeaponGauge.DOFillAmount(1, _assultReloadTime)
                        .SetEase(Ease.Linear))
                    .SetLink(this.gameObject);
                sequence.Join(_rocketLauncherBulletCircleGauge.DOFillAmount(1, _assultReloadTime)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() => UpdatePercentageText(WeaponUiState.RocketLauncher)).OnComplete(() =>
                    {
                        IsRocketLauncherReload = false;
                        _rocketLauncherCurrentCount = _rocketLauncherBulletMaxCount;
                    }).SetLink(this.gameObject)
                );

                sequence.SetAutoKill(true);

                // シーケンスを開始
                sequence.Play();
            }
        }
    }
}