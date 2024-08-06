using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class SideBarController : MonoBehaviour
{
    [Header("1ループする時間")]
    [SerializeField] private float _animationDuration = 1.0f;
    [Header("アニメーションさせるImageオブジェクト")]
    [SerializeField] private Image _gaugeImage;
    [Header("初期値のゲージ割合")] [SerializeField, Range(0, 1)]
    private float _defaultFiillAmount;
    [Header("ゲージの変化値")] [SerializeField, Range(-1, 1)]
    private float _changeAmount;
    
    /// <summary>
    /// 現在の値
    /// </summary>
    private float _currentValue;
    
    // Start is called before the first frame update
    private void Start()
    {
        //ゲージを初期化する
        _gaugeImage.fillAmount = _defaultFiillAmount;
        _currentValue = _defaultFiillAmount;
    }

    /// <summary>
    /// 無限ループでゲージの値をランダムに増減する
    /// </summary>
    public void StartGaugeAnimation()
    {
        var tween = DOTween.To(() => _currentValue, x => _currentValue = x, _currentValue + _changeAmount,
                _animationDuration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo)
            .OnUpdate(() =>
            {
                // fillAmountの範囲を0から1に制限
                _gaugeImage.fillAmount = Mathf.Clamp(_currentValue, 0f, 1f);
            })
            .SetLink(this.gameObject);
    }
}
