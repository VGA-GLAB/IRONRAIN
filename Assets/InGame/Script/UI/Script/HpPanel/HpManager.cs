using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

//public enum BodyState
//{
//    Head,
//    LeftArm,
//    RightArm,
//    LeftFoot,
//    RightFoot
//}
public class HpManager : MonoBehaviour
{
    [Header("体力ゲージ")]
    [SerializeField] private Image[] _hpGauge;
    [Header("体力テキスト")] 
    [SerializeField] private TextMeshProUGUI[] _bodyHpText;
    [Header("危険状態のロボットUi")] 
    [SerializeField] private GameObject[] _dangerRobotUi;
    [Header("危険状態に変えるパーセント")]
    [SerializeField] private int _dangerDamage = 30;
    [Header("通常時のテキストフレーム")]
    [SerializeField] private GameObject[] _defultTextFrame;
    [Header("危険時のテキストフレーム")]
    [SerializeField] private GameObject[] _dangerTextFrame;
    [Header("数値とゲージが変化する時間")]
    [SerializeField] float _animationDuration;
    /// <summary>各部位の損傷率 </summary>
    private int[] _damageArray;
    /// <summary>合計の損傷率 </summary>
    //int _totalDamage;
    /// <summary>危険状態の判定 </summary>
    //bool IsDanger = false;
    
    // [SerializeField, Tooltip("点滅させるパネル")] GameObject _panelObj;
    // [SerializeField, Tooltip("点滅させる最大のアルファ値")] float _panelA = 0.5f;
    // [SerializeField, Tooltip("点滅するインターバル")] float _flashInterval = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        _damageArray = new int[_hpGauge.Length];
        //各部位のTextMeshProと損傷率を初期化
        for (int i = 0;  i < _hpGauge.Length; i++)
        {
            _hpGauge[i].fillAmount = 1f;
            _damageArray[i] = 100;
            _bodyHpText[i].text = _damageArray[i] + "%";
            _dangerRobotUi[i].SetActive(false);
            _defultTextFrame[i].SetActive(true);
            _dangerTextFrame[i].SetActive(false);
        }
    }

    /// <summary>
    /// ダメージを受けたときの処理
    /// </summary>
    /// <param name="damage">損傷率</param>
    public void BodyDamage(int damage)
    {
        //すべての部位でダメージが耐えられない場合
        for (int i = 0; i < _damageArray.Length; i++)
        {
            if (_damageArray[i] - damage > 0)
                break;

            if (i == _damageArray.Length - 1)
                return;
        }

        while (true)
        {
            //損傷部位を取得
            int index = Random.Range(0, _damageArray.Length);
            int value = _damageArray[index] - damage;

            if (value > 0)
            {
                float hpPercent = value / 100f;
                //徐々に損傷部位の値を変える
                // DoTweenを使ってゲージのfillAmountをアニメーション
                _hpGauge[index].DOFillAmount(hpPercent, _animationDuration).SetEase(Ease.Linear)
                    .OnUpdate(() => UpdatePercentageText(index)).SetLink(this.gameObject);

                _damageArray[index] -= damage;

                //危険域に入った部位のUiとテキストフレームを切り替える
                if (_damageArray[index] <= _dangerDamage)
                {
                    _dangerRobotUi[index].SetActive(true);
                    _defultTextFrame[index].SetActive(false);
                    _dangerTextFrame[index].SetActive(true);
                }
                break;
            }   //損傷箇所が0以上になら
        }
    }

    /// <summary>
    /// ％テキストを更新する処理
    /// </summary>
    private void UpdatePercentageText(int index)
    {
        int percentage = Mathf.RoundToInt(_hpGauge[index].fillAmount * 100);
        _bodyHpText[index].text = percentage.ToString() + "%";
    }

    // /// <summary>
    // /// UIを赤く点滅させる
    // /// </summary>
    // void PanelFlash()
    // {
    //     //アラートを鳴らす
    //     CriAudioManager.Instance.SE.Play("SE", "SE_Alert");
    //     Image panel = _panelObj.GetComponent<Image>();
    //     panel.DOFade(_panelA, _flashInterval).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject);
    //     //DOTween.ToAlpha(() => panel.color, color => panel.color = color, _panelA, _flashInterval).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject);
    // }
}
