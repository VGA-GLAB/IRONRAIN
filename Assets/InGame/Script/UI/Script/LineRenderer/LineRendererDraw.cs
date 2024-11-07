using UnityEngine;

public class LineRendererDraw : MonoBehaviour
{
    [SerializeField, Tooltip("使用するLineRenderer")] private LineRenderer _lineRenderer;
    [SerializeField, Tooltip("Rayを飛ばすオブジェクト")] private GameObject _origin;
    //[SerializeField, Tooltip("Rayの長さ")] private float _rayDistance = 100f;
    [SerializeField, Tooltip("RayのLayerMask")] private LayerMask _layerMask;

    /// <summary>頂点の数 </summary>
    private int _posCount;
    /// <summary>頂点を生成する最低間隔 </summary>
    private float _interval = 0.1f;
    /// <summary>ボタンを押しているかのフラグ </summary>
    private bool _isInput;


    private void Start()
    {
        // InputProvider.Instance.SetEnterInput(InputProvider.InputType.LeftTrigger, LineStart);
        // InputProvider.Instance.SetExitInput(InputProvider.InputType.LeftTrigger, LineEnd);
    }

    private void Update()
    {
        if (!_isInput)
            return;

        var rayStartPosition = _origin.transform.position;
        //マウスでRayを飛ばす方向を決める
        var direction = _origin.transform.forward;
        //Hitしたオブジェクト格納用
        RaycastHit hit;
        if (Physics.Raycast(rayStartPosition, direction, out hit, Mathf.Infinity, _layerMask))
        {
            var pos = hit.point;
            //pos.z = _panel.transform.position.z;
            //SetPosition(Camera.main.ScreenToWorldPoint(pos));
            SetPosition(pos);
        }
    }

    // 線を伸ばすメソッド
    private void SetPosition(Vector3 pos)
    {
        if (!PosCheck(pos)) 
        {
            return;
        }
        _posCount++;
        _lineRenderer.positionCount = _posCount;
        _lineRenderer.SetPosition(_posCount - 1, pos);
            
    }

    //頂点を増やしてもよいかを判定するメソッド
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

    private void LineStart()
    {
        _isInput = true;
    }

    private void LineEnd()
    {
        //ラインレンダラーを初期化
        _lineRenderer.positionCount = 0;
        _posCount = 0;
        _isInput = false;
    }
}
