using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiZoom : MonoBehaviour
{
    [SerializeField, Tooltip("ズームしたいパネル")] private RectTransform _zoomPanel;
    [SerializeField, Tooltip("拡大が完了する時間")] private float _zoomTime;
    [SerializeField, Tooltip("拡大する大きさ")] private float _scale;
    [SerializeField, Tooltip("部位ごとのRectTransform")] private RectTransform[] _rectTransforms;
    [SerializeField, Tooltip("初期状態のパネル")] private RectTransform _normalPanelSize;
    /// <summary>パネルが現在ズーム状態かの判定 </summary>
    private bool _isZoom = false;

    public void OnButtom(int num)
    {
        //ズーム状態かにより、条件分岐
        //ズーム状態ならもとに戻し、それ以外ならズームする
        if (_isZoom)
            NormalUi();
        else
            ZoomUi(num);
        _isZoom = !_isZoom;
    }

    /// <summary>
    /// UIをズームするときに呼び出すメソッド
    /// </summary>
    /// <param name="num">部位の添え字</param>
    private void ZoomUi(int num)
    {
        //Sequenceの生成
        var sequence = DOTween.Sequence().SetLink(_zoomPanel.gameObject);
        //RectTransformでパネルを移動させる
        sequence.Append(_zoomPanel.DOAnchorPos(new Vector2(_rectTransforms[num].anchoredPosition.x, _rectTransforms[num].anchoredPosition.y), _zoomTime));
        //ズームさせる
        sequence.Join(_zoomPanel.DOScale(_scale, _scale));
        sequence.Play();
    }

    /// <summary>
    /// 通常のパネルに戻す
    /// </summary>
    private void NormalUi()
    {
        //Sequenceの生成
        var sequence = DOTween.Sequence().SetLink(_zoomPanel.gameObject);
        //RectTransformでパネルを移動させる
        sequence.Append(_zoomPanel.DOAnchorPos(new Vector2(_normalPanelSize.anchoredPosition.x, _normalPanelSize.anchoredPosition.y), _zoomTime));
        //スケールを元に戻す
        sequence.Join(_zoomPanel.DOScale(1f, 1f));
        sequence.Play();
    }
}
