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
    [Header("ActiveUi")] [SerializeField] private GameObject _activeUiObject;
    [SerializeField] private Image _activeUiBackGround;
    [SerializeField] private Image _activeUiButton;
    [SerializeField] private float _startAnimationDuration = 1f;

    [Header("アニメーション")] [SerializeField] private VideoPlayer _activeUiAnimation;

    [Header("起動シーケンスのタイムライン")] [SerializeField]
    private PlayableDirector _launchPlayableDirector;

    [Header("タイムラインが始まってから次のシーケンスに行くまで待つ秒数")] [SerializeField]
    private float _animationWait = 3f;
    // Start is called before the first frame update
    void Start()
    {
        var backGroundcolor = _activeUiBackGround.color;
        backGroundcolor.a = 0;
        _activeUiBackGround.color = backGroundcolor;
        
        var rawImageColor = _activeUiButton.color;
        rawImageColor.a = 0;
        _activeUiButton.color = rawImageColor;
        
        var buttonColor = _activeUiAnimation.gameObject.GetComponent<RawImage>().color;
        buttonColor.a = 0;
        _activeUiButton.color = buttonColor;
        
        if (_activeUiButton.TryGetComponent(out Collider collider))
        {
            // Colliderを非アクティブにする
            collider.enabled = false;
        }

        _activeUiAnimation.loopPointReached += StartLaunchTimeLine;

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
        //ButtonActive();
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
        //Debug.Log("終了");
    }

    private void StartLaunchTimeLine(VideoPlayer vp)
    {
        //Debug.Log("再生された");
        _activeUiObject.gameObject.SetActive(false);
        //_launchPlayableDirector.Play();
    }
}