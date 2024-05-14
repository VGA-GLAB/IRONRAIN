using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;

public class MultilockSystem : MonoBehaviour
{
    [SerializeField, Tooltip("Rayを飛ばす起点")] GameObject _rayOrigin;
    /// <summary>マルチロック中であるか </summary>
    public bool IsMultilock { get; private set; }
    /// <summary>敵のUIリスト </summary>
    public List<EnemyUi> EnemyUis;
    /// <summary>敵のUIリスト（テスト用） </summary>
    public List<GameObject> LockOnEnemy;
    [SerializeField, Tooltip("Rayのレイヤーマスク")] LayerMask _layerMask;
    
    // Update is called once per frame
    private void Update()
    {
        if(Input.GetMouseButtonDown(0)) //マウスを押した瞬間
        {
            IsMultilock = true;
        }
        else if(Input.GetMouseButton(0))//マウスが押されている間
        {

        }
        else if(Input.GetMouseButtonUp(0))//マウスが離れた時
        {
            IsMultilock = false;
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
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Hitしたオブジェクト格納用
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
        {
            if(hit.collider.gameObject.tag == "Enemy")
            {
                LockOnEnemy.Add(hit.collider.gameObject);
            }
        }
    }

    private void MultiLock()
    {
        LockOnEnemy = LockOnEnemy.Distinct().ToList();
    }
}
