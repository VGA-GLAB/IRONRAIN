using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField, Tooltip("体の部位")] GameObject[] _bodyList;
    [SerializeField, Tooltip("点滅させるパネル")] GameObject _panelObj;
    [SerializeField, Tooltip("点滅させる最大のアルファ値")] float _panelA = 0.5f;
    [SerializeField, Tooltip("点滅するインターバル")] float _flashInterval = 3.0f;
    [SerializeField, Tooltip("点滅させる損傷率")] int _flashDamage = 30;
    /// <summary>各部位の損傷率 </summary>
    int[] _damageArray;
    /// <summary>各部位のTextMeshPro </summary>
    TextMeshProUGUI[] _textMeshPros;
    /// <summary>合計の損傷率 </summary>
    int _totalDamage;
    /// <summary>危険状態の判定 </summary>
    bool IsDanger = false;
    // Start is called before the first frame update
    void Start()
    {
        _damageArray = new int[_bodyList.Length];
        _textMeshPros = new TextMeshProUGUI[_bodyList.Length];
        //各部位のTextMeshProと損傷率を初期化
        for (int i = 0;  i < _bodyList.Length; i++)
        {
            _textMeshPros[i] = _bodyList[i].GetComponent<TextMeshProUGUI>();
            _damageArray[i] = 100;
            _textMeshPros[i].text = _damageArray[i] + "%";
        }
    }

    // Update is called once per frame
    void Update()
    {
                  
    }

    /// <summary>
    /// ダメージを受けたときの処理
    /// </summary>
    /// <param name="damage">損傷率</param>
    public void BodyDamage(int damage)
    {
        while (true)
        {
            //損傷部位を取得
            int index = Random.Range(0, _damageArray.Length);
            int value = _damageArray[index] - damage;
            if (value > 0)
            {
                //徐々に損傷部位の値を変える
                DOVirtual.Int(_damageArray[index], value, 1, num =>
                {
                    _damageArray[index] = num;
                    _textMeshPros[index].text = _damageArray[index] + "%";
                }).SetLink(gameObject);
                //DOTween.To(() => _damageArray[index], num => _damageArray[index] -= num, value, 1.0f)
                //    .OnUpdate();
                _totalDamage += damage;
                //_damageArray[index] -= damage;
                //損傷率の判定をする
                if (_totalDamage >= _flashDamage && !IsDanger)
                {
                    //PanelFlash();
                    IsDanger = true;
                }
                break;
            }   //損傷箇所が0以上になら
        }
    }

    /// <summary>
    /// UIを赤く点滅させる
    /// </summary>
    //void PanelFlash()
    //{
    //    Image panel = _panelObj.GetComponent<Image>();
    //    //panel.DOFade(_panelA, _flashInterval).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject);
    //    DOTween.ToAlpha(() => panel.color, color => panel.color = color, _panelA, _flashInterval).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject);
    //}
}
