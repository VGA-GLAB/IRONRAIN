using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiZoom : MonoBehaviour
{
    [Header("ズームしたいパネル")]
    [SerializeField] private RectTransform _zoomPanel;
    [Header("拡大する時")]
    [Header("移動時間")]
    [SerializeField] private float _expansionMoveTime;
    [Header("拡大時間")]
    [SerializeField] private float _expansionZoomTime;
    [Header("拡大するスケール")]
    [SerializeField] private float _expansionScale;
    [Header("縮小する時")]
    [Header("移動時間")]
    [SerializeField] private float _reductionMoveTime;
    [Header("拡大時間")]
    [SerializeField] private float _reductionZoomTime;
    [Header("部位ごとのRectTransform")]
    [SerializeField] private RectTransform[] _rectTransforms;
    [Header("初期状態のパネル位置")]
    [SerializeField] private RectTransform _normalPanelPosition;
    [Header("初期状態のスケール")]
    [SerializeField] private float _normalSize;
    [Header("移動時のオフセット(0から頭、左腕、右腕、左足、右足)")]
    [SerializeField] private Vector2[] _uiOffset;
    [Header("ボタン")]
    [SerializeField] private GameObject[] _uiButton;
    [Header("VR用あたり判定")]
    [SerializeField] private GameObject[] _vrCollider;
    [Header("マウス用")]
    [SerializeField] private bool _isMouse = false;
    /// <summary>パネルが現在ズーム状態かの判定 </summary>
    private bool _isZoom = false;

    private void Start()
    {
        if(_isMouse)
        {
            for(int i = 0; i < _uiButton.Length; i++)
            {
                _uiButton[i].SetActive(true);
                _vrCollider[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < _uiButton.Length; i++)
            {
                _uiButton[i].SetActive(false);
                _vrCollider[i].SetActive(true);
            }
        }
    }

    public void OnButtomZoom(int num)
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
        sequence.Append(_zoomPanel.DOAnchorPos(new Vector2(-_rectTransforms[num].anchoredPosition.x - _uiOffset[num].x, -_rectTransforms[num].anchoredPosition.y -_uiOffset[num].y), _expansionMoveTime));
        //ズームさせる
        sequence.Join(_zoomPanel.DOScale(_expansionScale, _expansionZoomTime));
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
        sequence.Append(_zoomPanel.DOAnchorPos(new Vector2(_normalPanelPosition.anchoredPosition.x, _normalPanelPosition.anchoredPosition.y), _reductionMoveTime));
        //スケールを元に戻す
        sequence.Join(_zoomPanel.DOScale(_normalSize, _reductionZoomTime));
        sequence.Play();
    }
}
