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
    public HashSet<GameObject> LockOnEnemy = new HashSet<GameObject>();

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
                if (!LockOnEnemy.Contains(enemyUi.Enemy))
                {
                    //ターゲットをロックしたときに出す音
                    CriAudioManager.Instance.SE.Play("SE", "SE_Targeting");
                }
                LockOnEnemy.Add(enemyUi.Enemy);
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
            if (LockOnEnemy.Count > 0)
            {
                //LockOnEnemy = LockOnEnemy.Distinct().ToList();
                _raderMap.MultiLockon(LockOnEnemy);
            }
            IsMultilock = false;
            LockOnEnemy.Clear();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //パネルに触れた時の音
        CriAudioManager.Instance.SE.Play("SE", "SE_Panel_Tap");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsMultilock)
        {
            //多重ロックオン発動時に流れる音
            CriAudioManager.Instance.SE.Play("SE", "SE_Lockon");
            SerchEnemy();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EndMultilockAction();
        _lockUi.Clear();
    }

    public void EnemyDestory(GameObject enemy)
    {
        LockOnEnemy.Remove(enemy);
        _lockUi.Remove(enemy);
    }
}