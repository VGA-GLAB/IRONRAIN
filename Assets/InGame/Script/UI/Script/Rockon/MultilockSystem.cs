using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using Zinnia.Extension;

public class MultilockSystem : MonoBehaviour
{
    [SerializeField, Tooltip("Rayを飛ばす起点")] GameObject _rayOrigin;
    /// <summary>マルチロック中であるか </summary>
    public bool IsMultilock;
    /// <summary>敵のUIリスト </summary>
    public List<GameObject> LockOnEnemy;
    [SerializeField, Tooltip("Rayのレイヤーマスク")] LayerMask _layerMask;
    /// <summary>レーダーマップ </summary>
    private RaderMap _raderMap;

    private void Awake()
    {
        //レーダーテストを検索する
        _raderMap = GameObject.Find("RaderTest").GetComponent<RaderMap>();
        IsMultilock = true;
    }

    private void Start()
    {
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.LeftTrigger, MultilockOn);
        InputProvider.Instance.SetExitInput(InputProvider.InputType.LeftTrigger, MultilockAction);
    }

    // Update is called once per frame
    private void Update()
    {
        if(IsMultilock)
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
        //マウスでRayを飛ばす方向を決める
        var direction = _rayOrigin.transform.forward;
        //Hitしたオブジェクト格納用
        RaycastHit hit;
        if (Physics.Raycast(rayStartPosition, direction, out hit, Mathf.Infinity, _layerMask))
        {
            if (hit.collider.gameObject.TryGetComponent(out EnemyUi enemyUi))
            {
                LockOnEnemy.Add(enemyUi.Enemy);
            }
        }
    }

    private void MultilockAction()
    {
        //格納したエネミーで同じものを削除する
        LockOnEnemy = LockOnEnemy.Distinct().ToList();
        if (LockOnEnemy.Count > 0)
        {
            _raderMap.MultiLockon(LockOnEnemy);   
        }
        IsMultilock = false;
        LockOnEnemy.Clear();
        Debug.Log("マルチロック終了");
    }

    public void MultilockOn()
    {
        IsMultilock = true;
        Debug.Log("マルチロック開始");
    }

    public void OnDrag(PointerEventData eventData)
    {
        var gameObject = eventData.pointerCurrentRaycast.gameObject;
        if(gameObject.TryGetComponent(out EnemyUi enemyUi))
        {
            LockOnEnemy.Add(enemyUi.Enemy);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        MultilockAction();
        LockOnEnemy.Clear();
        Debug.Log("マルチロック終了");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(new Ray(_rayOrigin.transform.position, _rayOrigin.transform.forward));
    }
}
