using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class LineRendererDraw : MonoBehaviour
{
    [SerializeField, Tooltip("使用するLineRenderer")] private LineRenderer _lineRenderer;
    [SerializeField, Tooltip("Rayを飛ばすオブジェクト")] private GameObject _origin;
    [SerializeField, Tooltip("Rayの長さ")] private float _rayDistance = 100f;
    [SerializeField, Tooltip("RayのLayerMask")] private LayerMask _layerMask;
    [SerializeField, Tooltip("PanelのZ座標")] private GameObject _panel;

    /// <summary>頂点の数 </summary>
    private int _posCount = 0;
    /// <summary>頂点を生成する最低間隔 </summary>
    private float _interval = 0.1f;
    /// <summary>ボタンを押しているかのフラグ </summary>
    private bool IsInput = false;


    private void Start()
    {
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.LeftTrigger, LineStart);
        InputProvider.Instance.SetExitInput(InputProvider.InputType.LeftTrigger, LineEnd);
    }

    private void Update()
    {
        if (!IsInput)
            return;

        var rayStartPosition = _origin.transform.position;
        //マウスでRayを飛ばす方向を決める
        var direction = _origin.transform.forward;
        //Hitしたオブジェクト格納用
        RaycastHit hit;
        if (Physics.Raycast(rayStartPosition, direction, out hit, Mathf.Infinity, _layerMask))
        {
            var pos = hit.point;
            pos.z = _panel.transform.position.z;
            SetPosition(Camera.main.ScreenToWorldPoint(pos));
            Debug.Log("線を書く");
        }
    }

    /// <summary>
    /// 線を伸ばすメソッド
    /// </summary>
    /// <param name="pos">マウスの位置</param>
    private void SetPosition(Vector3 pos)
    {
        if (!PosCheck(pos)) 
        {
            Debug.Log("だめ");
            return;
        }
        _posCount++;
        _lineRenderer.positionCount = _posCount;
        _lineRenderer.SetPosition(_posCount - 1, pos);
            
    }

    /// <summary>
    /// 頂点を増やしてもよいかを判定するメソッド
    /// </summary>
    /// <param name="pos">現在のマウスポジション</param>
    /// <returns>頂点を増やすかの判定</returns>
    private bool PosCheck(Vector3 pos)
    {
        if (_posCount == 0)
            return true;

        float distance = Vector3.Distance(_lineRenderer.GetPosition(_posCount - 1), pos);
        if (distance > _interval)
            return true;
        else
            return false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 pos = eventData.position;
        pos.z = _panel.transform.position.z;
        SetPosition(Camera.main.ScreenToWorldPoint(pos));

        //if (Mouse.current.leftButton.IsPressed() || IsInput)
        //    //Rayを飛ばすスタート位置を決める
        //    var rayStartPosition = _origin.transform.position;
        ////マウスでRayを飛ばす方向を決める
        //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        ////Hitしたオブジェクト格納用
        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
        //{
        //    SetPosition(hit.point);
        //    Debug.Log("線を書く");
        //}
    }

    private void LineStart()
    {
        IsInput = true;
    }

    private void LineEnd()
    {
        //ラインレンダラーを初期化
        _lineRenderer.positionCount = 0;
        _posCount = 0;
        IsInput = false;
        Debug.Log("線を消す");
    }

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    //ラインレンダラーを初期化
    //    _lineRenderer.positionCount = 0;
    //    _posCount = 0;
    //    Debug.Log("線を消す");
    //}
}
