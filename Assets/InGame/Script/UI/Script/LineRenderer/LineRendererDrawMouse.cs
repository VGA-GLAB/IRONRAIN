using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace mouse
{
    public class LineRendererDraw : MonoBehaviour, IPointerDownHandler,IDragHandler, IPointerUpHandler
    {
        [SerializeField, Tooltip("使用するLineRenderer")] private LineRenderer _lineRenderer;
        [SerializeField, Tooltip("Rayを飛ばすオブジェクト")] private GameObject _origin;
        //[SerializeField, Tooltip("Rayの長さ")] private float _rayDistance = 100f;
        [SerializeField, Tooltip("RayのLayerMask")] private LayerMask _layerMask;
        private MouseMultiLockSystem _mouseMultiLockSystem;

        // 頂点の数 
        private int _posCount = 0;
        // 頂点を生成する最低間隔 
        private float _interval = 0.1f;
        /// <summary>ボタンを押しているかのフラグ </summary>
        //private bool IsInput = false;

        private void Start()
        {
            _mouseMultiLockSystem = FindObjectOfType(typeof(MouseMultiLockSystem)).GetComponent<MouseMultiLockSystem>();
        }
        public void OnDrag(PointerEventData eventData)
        {
            var rayStartPosition = _origin.transform.position;
            var mousePos = Input.mousePosition;
            mousePos.z = 1f;
            //マウスでRayを飛ばす方向を決める
            var worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
            var direction = (worldMousePos - rayStartPosition).normalized;
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

        public void OnPointerUp(PointerEventData eventData)
        {
            LineEnd();
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

        // 頂点を増やしてもよいかを判定するメソッド
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
            //IsInput = true;
        }

        private void LineEnd()
        {
            //ラインレンダラーを初期化
            _lineRenderer.positionCount = 0;
            _posCount = 0;
            //IsInput = false;
        }

        public void OnPointerDown(PointerEventData eventData) { }
    }
}