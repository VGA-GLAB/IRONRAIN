using System;
using System.Collections.Generic;
using System.Linq;
using Enemy.Control;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

public class MouseMultilockSystem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField, Tooltip("Rayを飛ばす起点")] GameObject _rayOrigin;

    /// <summary>マルチロック中であるか </summary>
    public bool IsMultilock;

    /// <summary>敵のUIリスト </summary>
    private HashSet<GameObject> _lockOnEnemy = new HashSet<GameObject>();

    [SerializeField, Tooltip("Rayのレイヤーマスク")]
    LayerMask _layerMask;
    [SerializeField, Tooltip("使用するLineRenderer")] private LineRenderer _lineRenderer;
    [SerializeField, Tooltip("Rayの距離")] private float _rayDis = 100f;
    /// <summary>レーダーマップ </summary>
    private RaderMap _raderMap;
    /// <summary>ロックオンしたUi </summary>
    private HashSet<GameObject> _lockUi = new HashSet<GameObject>();
    private int _posCount;

    private void Awake()
    {
        //レーダーテストを検索する
        _raderMap = GameObject.FindObjectOfType(typeof(RaderMap)).GetComponent<RaderMap>();
    }

    private void LateUpdate()
    {
        _posCount = 0;
        _lineRenderer.positionCount = _posCount;
        //ラインレンダラーの更新
        if (_lockUi.Count < 1)
            return;
        _lineRenderer.positionCount = _lockUi.Count;

        foreach (GameObject obj in _lockUi)
        {
            _posCount++;
            _lineRenderer.positionCount = _posCount;
            _lineRenderer.SetPosition(_posCount - 1, obj.transform.position);
        }
    }
    /// <summary>
    /// エネミーを探す処理
    /// </summary>
    private void SerchEnemy()
    {
        //Rayを飛ばすスタート位置を決める
        var rayStartPosition = _rayOrigin.transform.position;
        var mousePos = Input.mousePosition;
        mousePos.z = 1f;
        //マウスでRayを飛ばす方向を決める
        var worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        var direction = (worldMousePos - rayStartPosition).normalized;
        //Hitしたオブジェクト格納用
        RaycastHit hit;
        if (Physics.Raycast(rayStartPosition, direction, out hit, _rayDis, _layerMask))
        {
            //Debug.Log("当たった");
            if (hit.collider.gameObject.TryGetComponent(out EnemyUi enemyUi))
            {
                //Debug.Log("uiに当たった");
                _lockOnEnemy.Add(enemyUi.Enemy);
                _lockUi.Add(enemyUi.gameObject);
            }
        }
        
    }

    /// <summary>
    /// マルチロックのスタート時に呼ばれる
    /// </summary>
    public void MultilockOnStart()
    {
        IsMultilock = true;
    }
    
    /// <summary>
    /// マルチロックの終了時に呼ばれる
    /// </summary>
    private void EndMultilockAction()
    {
        if (IsMultilock)
        {
            //格納したエネミーで同じものを削除する
            if (_lockOnEnemy.Count > 0)
            {
                //_lockOnEnemy = _lockOnEnemy.Distinct().ToList();
                _raderMap.MultiLockon(_lockOnEnemy);
            }
            IsMultilock = false;
            _lockOnEnemy.Clear();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsMultilock)
        {
            SerchEnemy();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(_lockOnEnemy.Count > 1)
            EndMultilockAction();
        else
            _lockOnEnemy.Clear();

        _lockUi.Clear();
    }

    public void EnemyDestory(GameObject enemy)
    {
        _lockOnEnemy.Remove(enemy);
        _lockUi.Remove(enemy);
    }
}