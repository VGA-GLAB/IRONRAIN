using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;

public class LaunchManager : MonoBehaviour
{
    [Header("テストフラグ")]
    [SerializeField]
    private bool _isTest;
    [Header("ActiveUi")] [SerializeField] private GameObject _activeUiObject;
    [SerializeField] private Image _activeUiBackGround;
    [SerializeField] private Image _activeUiButton;
    [Header("ActiveUiのアニメーション秒数")]
    [SerializeField] private float _startAnimationDuration = 1f;

    [Header("アニメーション")] [SerializeField] private VideoPlayer _activeUiAnimation;

    [Header("起動シーケンスのタイムライン")] [SerializeField]
    private PlayableDirector _launchPlayableDirector;

    [Header("それぞれのアニメーション")] [Header("機体")] 
    [SerializeField] private VideoPlayer _detailUiAnimation;
    [Header("マップ")][SerializeField] private VideoPlayer _mapUiAnimation;
    [Header("アナウンス")] [SerializeField] private VideoPlayer _announceUiAnimation;
    [Header("武器")] [SerializeField] private VideoPlayer _weaponUiAnimation;
    
    [Header("タイムラインが始まってから次のシーケンスに行くまで待つ秒数")] [SerializeField]
    private float _animationWait = 3f;

    private bool _isActivate;

    [Header("最初に透明にしたいオブジェクト")]
    [Header("起動Ui")]
    [SerializeField]private CanvasGroup _launcherUi;
    [Header("武器Ui２種")]
    [SerializeField]private CanvasGroup _assultUi;
    [SerializeField]private CanvasGroup _rocketLauncherUi;
    [Header("アナウンス")]
    [SerializeField]private CanvasGroup _announceUi;
    [Header("詳細Ui")]
    [SerializeField]private CanvasGroup _detailUi;
    [Header("ミニマップ")]
    [SerializeField]private CanvasGroup _minimapUi;

    [Header("アニメーションを再生する間隔")] [SerializeField]
    private float _animationInterval = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        _launcherUi.alpha = 0;
        _assultUi.alpha = 0;
        _rocketLauncherUi.alpha = 0;
        _announceUi.alpha = 0;
        _detailUi.alpha = 0;
        _minimapUi.alpha = 0;

        var backGroundcolor = _activeUiBackGround.color;
        backGroundcolor.a = 0;
        _activeUiBackGround.color = backGroundcolor;
        
        var rawImageColor = _activeUiButton.color;
        rawImageColor.a = 0;
        _activeUiButton.color = rawImageColor;
        
        //videoplayerのrawImageを透明にする
        var activeRowImage = _activeUiAnimation.gameObject.GetComponent<RawImage>().color;
        activeRowImage.a = 0;
        _activeUiAnimation.gameObject.GetComponent<RawImage>().color = activeRowImage;

        var detailUiAnimationrowImage = _detailUiAnimation.gameObject.GetComponent<RawImage>().color;
        detailUiAnimationrowImage.a = 0;
        _detailUiAnimation.gameObject.GetComponent<RawImage>().color = detailUiAnimationrowImage;

        var mapUiAnimationRowImage = _mapUiAnimation.gameObject.GetComponent<RawImage>().color;
        mapUiAnimationRowImage.a = 0;
        _mapUiAnimation.gameObject.GetComponent<RawImage>().color = mapUiAnimationRowImage;

        var announceUiAnimationRowImage = _announceUiAnimation.gameObject.GetComponent<RawImage>().color;
        announceUiAnimationRowImage.a = 0;
        _announceUiAnimation.gameObject.GetComponent<RawImage>().color = announceUiAnimationRowImage;

        var weaponUiAnimationRowImage = _weaponUiAnimation.gameObject.GetComponent<RawImage>().color;
        weaponUiAnimationRowImage.a = 0;
        _weaponUiAnimation.gameObject.GetComponent<RawImage>().color = weaponUiAnimationRowImage;

        if (_activeUiButton.TryGetComponent(out Collider collider))
        {
            // Colliderを非アクティブにする
            collider.enabled = false;
        }

        _activeUiAnimation.loopPointReached += StartLaunchTimeLine;
        _mapUiAnimation.loopPointReached += ActiveLauch;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            StartActivateUi();
        }
    }

    /// <summary>
    /// Activate Animationをスタートさせる
    /// </summary>
    /// <param name="token"></param>
    public async UniTask StartActivateUi(CancellationToken token = default)
    {
        //見えるように変化させる
        Sequence startSequence = DOTween.Sequence();
        startSequence.Join(_activeUiBackGround.DOFade(1f, _startAnimationDuration)); // アルファ値1は255の意味
        startSequence.Join(_activeUiButton.DOFade(1f, _startAnimationDuration));
        
        await startSequence.Play().AsyncWaitForCompletion();
        startSequence.Kill();
        
        //テスト用
        if(_isTest)
        {
            ButtonActive();
        }
    }

    /// <summary>
    /// ボタンを押せるようにする処理
    /// </summary>
    public void ButtonActive()
    {
        if (_activeUiButton.TryGetComponent(out Collider collider))
        {
            // Colliderを非アクティブにする
            collider.enabled = true;
        }

        if (_activeUiButton.gameObject.TryGetComponent(out ActivationButtonCotroller button))
        {
            button._isButtonActive = true;
        }
    }

    /// <summary>
    /// 起動シーケンスアニメーションが終わるのを待つ
    /// </summary>
    /// <param name="ct"></param>
    public async UniTask WaitLaunchAnimation(CancellationToken ct)
    {
        await UniTask.WaitUntil(() => _isActivate, cancellationToken: ct);
    }
    /// <summary>
    /// 起動シーケンスのアニメーションをスタートさせる
    /// </summary>
    /// <param name="token"></param>
    public async UniTask StartLaunchSequence(CancellationToken token = default)
    {
        var buttonColor = _activeUiAnimation.gameObject.GetComponent<RawImage>().color;
        buttonColor.a = 1;
        _activeUiAnimation.gameObject.GetComponent<RawImage>().color = buttonColor;
        //アニメーション再生
        _activeUiAnimation.Play();
        _activeUiBackGround.gameObject.SetActive(false);
        _activeUiButton.gameObject.SetActive(false);
        
        await UniTask.Delay(TimeSpan.FromSeconds(_animationWait));
        _isActivate = true;
        //Debug.Log("終了");
    }

    private void StartLaunchTimeLine(VideoPlayer vp)
    {
        //Debug.Log("再生された");
        _activeUiObject.gameObject.SetActive(false);
        var buttonColor = _mapUiAnimation.gameObject.GetComponent<RawImage>().color;
        buttonColor.a = 1;
        _mapUiAnimation.gameObject.GetComponent<RawImage>().color = buttonColor;
        _mapUiAnimation.Play();
    }

    private void ActiveLauch(VideoPlayer vp)
    {
        _launchPlayableDirector.Play();
    }

    public void CallStartLaunchUiAnimation()
    {
        StartCoroutine(StartLaunchUiAnimation());
    }
    private  IEnumerator StartLaunchUiAnimation()
    {
        _detailUiAnimation.loopPointReached += (vp) => FinishAnimation(vp, _detailUi, _detailUiAnimation);
        _announceUiAnimation.loopPointReached += (vp) => FinishAnimation(vp, _announceUi, _announceUiAnimation);
        _weaponUiAnimation.loopPointReached += (vp) => FinishAnimation(vp, _assultUi, _rocketLauncherUi, _weaponUiAnimation);
        
        var detailColor = _detailUiAnimation.gameObject.GetComponent<RawImage>().color;
        detailColor.a = 1;
        _detailUiAnimation.gameObject.GetComponent<RawImage>().color = detailColor;
        //アニメーション再生
        _detailUiAnimation.Play();
        
        yield return new WaitForSeconds(_animationInterval);
        
        var announcelColor = _announceUiAnimation.gameObject.GetComponent<RawImage>().color;
        announcelColor.a = 1;
        _announceUiAnimation.gameObject.GetComponent<RawImage>().color = announcelColor;
        //アニメーション再生
        _announceUiAnimation.Play();
        
        yield return new WaitForSeconds(_animationInterval);
        
        var weaponColor = _weaponUiAnimation.gameObject.GetComponent<RawImage>().color;
        weaponColor.a = 1;
        _weaponUiAnimation.gameObject.GetComponent<RawImage>().color = weaponColor;
        //アニメーション再生
        _weaponUiAnimation.Play();
    }

    private void FinishAnimation(VideoPlayer vp, CanvasGroup canvasGroup, VideoPlayer videoPlayer)
    {
        canvasGroup.alpha = 1f;
        videoPlayer.gameObject.SetActive(false);
    }

    private void FinishAnimation(VideoPlayer vp, CanvasGroup canvasGroup0, CanvasGroup canvasGroup1, VideoPlayer videoPlayer)
    {
        canvasGroup0.alpha = 1f;
        canvasGroup1.alpha = 1f;
        videoPlayer.gameObject.SetActive(false);
    }
}