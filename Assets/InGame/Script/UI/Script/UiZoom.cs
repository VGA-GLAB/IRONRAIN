using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiZoom : MonoBehaviour
{
    [SerializeField, Tooltip("ズームしたいパネル")] GameObject _zoomPanel;
    [SerializeField, Tooltip("拡大が完了する時間")] float _zoomTime;
    [SerializeField, Tooltip("拡大する大きさ")] float _scale;
    [SerializeField, Tooltip("部位ごとのRectTransform")] RectTransform[] _rectTransforms;

    /// <summary>
    /// UIをズームするときに呼び出すメソッド
    /// </summary>
    /// <param name="num">部位の添え字</param>
    public void ZoomUi(int num)
    {
        
    }
}
