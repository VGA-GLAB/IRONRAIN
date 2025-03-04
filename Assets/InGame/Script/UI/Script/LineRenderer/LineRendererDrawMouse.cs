﻿using Unity.VisualScripting;
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
        private MouseMultilockSystem _mouseMultilockSystem;

        /// <summary>頂点の数 </summary>
        private int _posCount = 0;
        /// <summary>頂点を生成する最低間隔 </summary>
        private float _interval = 0.1f;
        /// <summary>ボタンを押しているかのフラグ </summary>
        //private bool IsInput = false;

        private void Start()
        {
            _mouseMultilockSystem = FindObjectOfType(typeof(MouseMultilockSystem)).GetComponent<MouseMultilockSystem>();
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
                //Debug.Log("線を書く");
            }
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            LineEnd();
        }

        /// <summary>
        /// 線を伸ばすメソッド
        /// </summary>
        /// <param name="pos">マウスの位置</param>
        private void SetPosition(Vector3 pos)
        {
            if (!PosCheck(pos))
            {
                //Debug.Log("だめ");
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
           // Debug.Log("線を消す");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }
    }
}

