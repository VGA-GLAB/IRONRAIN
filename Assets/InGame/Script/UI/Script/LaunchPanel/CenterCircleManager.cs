using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CenterCircleManager : MonoBehaviour
{
    [Header("内側の線")] [SerializeField] private RectTransform _insideLine;
    [SerializeField, Tooltip("左回転フラグ")] private bool _isInSideLeftRotate;
    [Header("1回転する秒数")] [SerializeField] private float _insideRotateTime = 2.0f;
    [Header("外側の線")] [SerializeField] private RectTransform _outsideLine;
    [SerializeField, Tooltip("左回転フラグ")] private bool _isOutSideLeftRotate;
    [Header("1回転する秒数")] [SerializeField] private float _outsideRotateTime = 2.0f;
    
    [Header("％テキストを入れるオブジェクト")]
    [SerializeField] private Text _percentText;
    [Header("動かす時間")]
    [SerializeField] private float _gaugeAnimationDuration = 5.0f;
    [Header("メモリリスト")]
    [SerializeField] private List<GameObject> _memoryList;

    private int _memoryCount;
    private int _memoryInterval;

    private void Start()
    {
        //文字の初期化
        _percentText.text = "0%";
        //ゲージの初期化
        for (int i = 0; i < _memoryList.Count; i++)
        {
            if(i == 0)
                _memoryList[i].SetActive(true);
            else
                _memoryList[i].SetActive(false);
        }
        
        _memoryCount = 1;
        _memoryInterval = 100 / _memoryList.Count;
    }
    
    /// <summary>
    /// アニメーションをアクティブにする
    /// </summary>
    public void ActiveAnimation()
    {
        ActiveGaugeAnimation();
        InsideLineAnimation();
        OutsideLineAnimation();
    }

    /// <summary>
    /// ゲージのアニメーション
    /// </summary>
    private void ActiveGaugeAnimation()
    {
        DOTween.To(() => 0, x => UpdatePercentageText(x), 100, _gaugeAnimationDuration)
            .SetEase(Ease.Linear).SetLink(_percentText.gameObject);
    }
    
    /// <summary>
    /// テキストの更新
    /// </summary>
    /// <param name="value"></param>
    private void UpdatePercentageText(int value)
    {
        _percentText.text = value.ToString() + "%";
        if (_memoryCount == _memoryList.Count - 1 && value == 100)
        {
            _memoryList[_memoryCount - 1].SetActive(false);
            _memoryList[_memoryCount].SetActive(true);
        }
        else if (_memoryInterval * _memoryCount <= value && _memoryCount != _memoryList.Count - 1)
        {
            _memoryList[_memoryCount - 1].SetActive(false);
            _memoryList[_memoryCount].SetActive(true);
            _memoryCount++;
        }
    }
    
    /// <summary>
    /// 内側の線のアニメーション
    /// </summary>
    private void InsideLineAnimation()
    {
        if (!_isInSideLeftRotate)
            ForwardRotateAnimation(_insideLine, _insideRotateTime);
        else
            ReverseRotateAnimation(_insideLine, _insideRotateTime);
    }

    /// <summary>
    /// 外側の線のアニメーション
    /// </summary>
    private void OutsideLineAnimation()
    {
        if (!_isOutSideLeftRotate)
            ForwardRotateAnimation(_outsideLine, _outsideRotateTime);
        else
            ReverseRotateAnimation(_outsideLine, _outsideRotateTime);
    }

    /// <summary>
    /// 順転回転のアニメーション
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="time"></param>
    private void ForwardRotateAnimation(RectTransform rectTransform, float time)
    {
        // 回転アニメーションを無限ループさせる
        rectTransform.DORotate(new Vector3(0, 0, 360), time, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart).SetLink(rectTransform.gameObject);
    }

    /// <summary>
    /// 逆回転のアニメーション
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="time"></param>
    private void ReverseRotateAnimation(RectTransform rectTransform, float time)
    {
        // 回転アニメーションを無限ループさせる
        rectTransform.DORotate(new Vector3(0, 0, -360), time, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart).SetLink(rectTransform.gameObject);
    }
}
