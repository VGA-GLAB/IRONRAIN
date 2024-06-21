using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseMultilockSystem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField, Tooltip("Rayを飛ばす起点")] GameObject _rayOrigin;

    /// <summary>マルチロック中であるか </summary>
    public bool IsMultilock;

    /// <summary>敵のUIリスト </summary>
    private List<GameObject> LockOnEnemy = new List<GameObject>();

    [SerializeField, Tooltip("Rayのレイヤーマスク")]
    LayerMask _layerMask;

    /// <summary>レーダーマップ </summary>
    private RaderMap _raderMap;

    private void Awake()
    {
        //レーダーテストを検索する
        _raderMap = GameObject.FindObjectOfType(typeof(RaderMap)).GetComponent<RaderMap>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsMultilock)
        {
            SerchEnemy();
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
        if (Physics.Raycast(rayStartPosition, direction, out hit, Mathf.Infinity, _layerMask))
        {
            //Debug.Log("当たった");
            if (hit.collider.gameObject.TryGetComponent(out EnemyUi enemyUi))
            {
                //Debug.Log("uiに当たった");
                LockOnEnemy.Add(enemyUi.Enemy);
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
                LockOnEnemy = LockOnEnemy.Distinct().ToList();
                _raderMap.MultiLockon(LockOnEnemy);
            }

            IsMultilock = false;
            LockOnEnemy.Clear();
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
        EndMultilockAction();
    }
}