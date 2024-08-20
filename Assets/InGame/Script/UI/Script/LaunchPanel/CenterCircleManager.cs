using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Oculus.Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CenterCircleManager : MonoBehaviour
{
    [Header("メモリが100%になる時間")]
    [SerializeField] private float _memoryCompleteTime = 10f;
    [Header("100%後に下ろすまでの時間")]
    [SerializeField] private float _downUiInterval = 0.5f;
    [Header("回転時間")]
    [SerializeField] private float _rotateAnimationTime = 10f;
    [Header("Uiを下に下ろす時間")]
    [SerializeField] private float _downUiTime = 0.5f;
    private bool _isFinish = false;

    [Header("内側の青線")]
    [SerializeField] private RectTransform _insideBlueLine;
    [SerializeField, Tooltip("左回転フラグ")] private bool _isInsideBlueLeftRotate;
    [Header("1回転する秒数")][SerializeField] private float _insideBlueRotateTime = 2.0f;
    [Header("上に移動するアニメーション")]
    [Header("移動を開始する時間")]
    [SerializeField] private float _insideBlueUpStartTime = 1.5f;
    [Header("移動時間")]
    [SerializeField] private float _insideBlueUpMoveTime = 0.2f;
    [Header("移動する高さ")]
    [SerializeField] private float _insideBlueUpHight = 0.2f;


    [Header("真ん中の青線")]
    [SerializeField] private RectTransform _centerBlueLine;
    [SerializeField, Tooltip("左回転フラグ")] private bool _isCenterBlueLeftRotate;
    [Header("1回転する秒数")][SerializeField] private float _insideCenterRotateTime = 2.0f;
    [Header("上に移動するアニメーション")]
    [Header("移動を開始する時間")]
    [SerializeField] private float _centerBlueUpStartTime = 3.5f;
    [Header("移動時間")]
    [SerializeField] private float _centerBlueUpMoveTime = 0.2f;
    [Header("移動する高さ")]
    [SerializeField] private float _centerBlueUpHight = 0.175f;

    [Header("内側の白線")] [SerializeField] private RectTransform _insideWhiteLine;
    [SerializeField, Tooltip("左回転フラグ")] private bool _isInSideWhiteLeftRotate;
    [Header("1回転する秒数")] [SerializeField] private float _insideWhiteRotateTime = 2.0f;
    [Header("上に移動するアニメーション")]
    [Header("移動を開始する時間")]
    [SerializeField] private float _insideWhiteUpStartTime = 5.5f;
    [Header("移動時間")]
    [SerializeField] private float _insideWhiteUpMoveTime = 0.2f;
    [Header("移動する高さ")]
    [SerializeField] private float _insideWhiteUpHight = 0.15f;

    [Header("外側の青線")]
    [SerializeField] private RectTransform _outsideBlueLine;
    [SerializeField, Tooltip("左回転フラグ")] private bool _isOutsideBlueLeftRotate;
    [Header("1回転する秒数")][SerializeField] private float _outsideCenterRotateTime = 2.0f;
    [Header("上に移動するアニメーション")]
    [Header("移動を開始する時間")]
    [SerializeField] private float _outsideBlueUpStartTime = 7.5f;
    [Header("移動時間")]
    [SerializeField] private float _outsideBlueUpMoveTime = 0.2f;
    [Header("移動する高さ")]
    [SerializeField] private float _outsideBlueUpHight = 0.125f;

    [Header("外側の白線")] [SerializeField] private RectTransform _outsideWhiteLine;
    [SerializeField, Tooltip("左回転フラグ")] private bool _isOutsideWhiteLeftRotate;
    [Header("1回転する秒数")] [SerializeField] private float _outsideWhiteRotateTime = 2.0f;
    [Header("上に移動するアニメーション")]
    [Header("移動を開始する時間")]
    [SerializeField] private float _outsideWhiteUpStartTime = 9.5f;
    [Header("移動時間")]
    [SerializeField] private float _outsideWhiteUpMoveTime = 0.2f;
    [Header("移動する高さ")]
    [SerializeField] private float _outsideWhiteUpHight = 0.1f;

    [Header("真ん中のゲージ")]
    [Header("％テキストを入れるオブジェクト")]
    [SerializeField] private TextMeshProUGUI _percentText;
    [Header("メモリリスト")]
    [SerializeField] private List<GameObject> _memoryList;

    private int _memoryCount;
    private int _memoryInterval;

    private void Awake()
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
        StartCoroutine(InsideWhiteLineAnimation());
        StartCoroutine(OutsideWhiteLineAnimation());
        StartCoroutine(InsideBlueLineAnimation());
        StartCoroutine(CenterBlueLineAnimation());
        StartCoroutine(OutsideBlueLineAnimation());
    }

    /// <summary>
    /// ゲージのアニメーション
    /// </summary>
    private void ActiveGaugeAnimation()
    {
        DOTween.To(() => 0, x => UpdatePercentageText(x), 100, _memoryCompleteTime)
            .SetEase(Ease.Linear).OnComplete(() => _isFinish = true).SetLink(_percentText.gameObject);
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
    /// 内側の白線のアニメーション
    /// </summary>
    private IEnumerator InsideWhiteLineAnimation()
    {
        if (!_isInSideWhiteLeftRotate)
            StartCoroutine(ForwardRotateAnimation(_insideWhiteLine, _insideWhiteRotateTime));
        else
            StartCoroutine(ReverseRotateAnimation(_insideWhiteLine, _insideWhiteRotateTime));

        yield return new WaitForSeconds(_insideWhiteUpStartTime);

        MoveUpUi(_insideWhiteLine, _insideWhiteUpHight, _insideWhiteUpMoveTime);

        yield return new WaitUntil(() => _isFinish);

        yield return new WaitForSeconds(_downUiInterval);

        MoveNormalUi(_insideWhiteLine, _insideWhiteUpHight, _downUiTime);
    }

    /// <summary>
    /// 外側の白線のアニメーション
    /// </summary>
    private IEnumerator OutsideWhiteLineAnimation()
    {
        if (!_isOutsideWhiteLeftRotate)
            StartCoroutine(ForwardRotateAnimation(_outsideWhiteLine, _outsideWhiteRotateTime));
        else
            StartCoroutine(ReverseRotateAnimation(_outsideWhiteLine, _outsideWhiteRotateTime));
        
        yield return new WaitForSeconds(_outsideWhiteUpStartTime);

        MoveUpUi(_outsideWhiteLine, _outsideWhiteUpHight, _outsideWhiteUpMoveTime);

        yield return new WaitUntil(() => _isFinish);

        yield return new WaitForSeconds(_downUiInterval);

        MoveNormalUi(_outsideWhiteLine, _outsideWhiteUpHight, _downUiTime);
    }

    /// <summary>
    /// 内側の青線のアニメーション
    /// </summary>
    private IEnumerator InsideBlueLineAnimation()
    {
        if (!_isInsideBlueLeftRotate)
            StartCoroutine(ForwardRotateAnimation(_insideBlueLine, _insideBlueRotateTime));
        else
            StartCoroutine(ReverseRotateAnimation(_insideBlueLine, _insideBlueRotateTime));
        
        yield return new WaitForSeconds(_insideBlueUpStartTime);

        MoveUpUi(_insideBlueLine, _insideBlueUpHight, _insideBlueUpMoveTime);

        yield return new WaitUntil(() => _isFinish);

        yield return new WaitForSeconds(_downUiInterval);

        MoveNormalUi(_insideBlueLine, _insideBlueUpHight, _downUiTime);
    }

    /// <summary>
    /// 内側の青線のアニメーション
    /// </summary>
    private IEnumerator CenterBlueLineAnimation()
    {
        if (!_isCenterBlueLeftRotate)
            StartCoroutine(ForwardRotateAnimation(_centerBlueLine, _insideCenterRotateTime));
        else
            StartCoroutine(ReverseRotateAnimation(_centerBlueLine, _insideCenterRotateTime));
        
        yield return new WaitForSeconds(_centerBlueUpStartTime);

        MoveUpUi(_centerBlueLine, _centerBlueUpHight, _centerBlueUpMoveTime);

        yield return new WaitUntil(() => _isFinish);

        yield return new WaitForSeconds(_downUiInterval);

        MoveNormalUi(_centerBlueLine, _centerBlueUpHight, _downUiTime);
    }

    /// <summary>
    /// 内側の青線のアニメーション
    /// </summary>
    private IEnumerator OutsideBlueLineAnimation()
    {
        if (!_isOutsideBlueLeftRotate)
            StartCoroutine(ForwardRotateAnimation(_outsideBlueLine, _outsideCenterRotateTime));
        else
            StartCoroutine(ReverseRotateAnimation(_outsideBlueLine, _outsideCenterRotateTime));
        
        yield return new WaitForSeconds(_outsideBlueUpStartTime);

        MoveUpUi(_outsideBlueLine, _outsideBlueUpHight, _outsideBlueUpMoveTime);

        yield return new WaitUntil(() => _isFinish);

        yield return new WaitForSeconds(_downUiInterval);

        MoveNormalUi(_outsideBlueLine, _outsideBlueUpHight, _downUiTime);
    }

    /// <summary>
    /// 順転回転のアニメーション
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="time"></param>
    private IEnumerator ForwardRotateAnimation(RectTransform rectTransform, float time)
    {
        // 回転アニメーションを無限ループさせる
        var tween = rectTransform.DORotate(new Vector3(0, 0, 360), time, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart).SetLink(rectTransform.gameObject);

        yield return new WaitForSeconds(_rotateAnimationTime);
        tween.Kill();
    }

    /// <summary>
    /// 逆回転のアニメーション
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="time"></param>
    private IEnumerator ReverseRotateAnimation(RectTransform rectTransform, float time)
    {
        // 回転アニメーションを無限ループさせる
        var tween = rectTransform.DORotate(new Vector3(0, 0, -360), time, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart).SetLink(rectTransform.gameObject);

        yield return new WaitForSeconds(_rotateAnimationTime);
        tween.Kill();
    }

    /// <summary>
    /// Uiを上げるアニメーション
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="hight"></param>
    /// <param name="time"></param>
    private void MoveUpUi(RectTransform rectTransform, float hight, float time)
    {
        // 現在のアンカー位置を取得
        Vector3 currentPos = rectTransform.anchoredPosition3D;
        rectTransform.DOAnchorPos3D(new Vector3(currentPos.x, currentPos.y, currentPos.z - hight), time)
            .SetEase(Ease.Linear).SetLink(rectTransform.gameObject);
    }

    /// <summary>
    /// 元の位置に戻す
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="hight"></param>
    /// <param name="time"></param>
    private void MoveNormalUi(RectTransform rectTransform, float hight, float time)
    {
        // 現在のアンカー位置を取得
        Vector3 currentPos = rectTransform.anchoredPosition3D;

        rectTransform.DOAnchorPos3D(new Vector3(currentPos.x, currentPos.y, currentPos.z + hight), time)
            .SetEase(Ease.Linear).SetLink(rectTransform.gameObject);
    }
}
