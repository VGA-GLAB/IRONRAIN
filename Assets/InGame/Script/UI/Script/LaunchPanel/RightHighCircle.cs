using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightHighCircle : MonoBehaviour
{
    [Header("％テキストを入れるオブジェクト")]
    [SerializeField] private Text _percentText;
    [Header("動かす時間")]
    [SerializeField] private float _animationDuration = 2.0f;
    [Header("アニメーションさせるImageオブジェクト")]
    [SerializeField] private Image _gaugeImage;

    [Header("変化後の%")] [SerializeField, Range(0, 1)]
    private float _targetFiillAmount;

    
    private void Awake()
    {
        //ゲージを初期化
        _gaugeImage.fillAmount = 0.0f;
        //文字の初期化
        _percentText.text = "0%";
    }
    /// <summary>
    /// ゲージを動かす処理
    /// </summary>
    public void ActiveGauge()
    {
        // DoTweenを使ってゲージのfillAmountをアニメーション
        _gaugeImage.DOFillAmount(_targetFiillAmount, _animationDuration).SetEase(Ease.Linear)
            .OnUpdate(UpdatePercentageText).SetLink(this.gameObject);
    }

    /// <summary>
    /// ％テキストを更新する処理
    /// </summary>
    private void UpdatePercentageText()
    {
        int percentage = Mathf.RoundToInt(_gaugeImage.fillAmount * 100);
        _percentText.text = percentage.ToString() + "%";
    }
}
