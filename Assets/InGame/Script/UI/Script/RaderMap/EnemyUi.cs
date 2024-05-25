using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyUi : MonoBehaviour, IPointerDownHandler
{
    public GameObject Enemy;
    /// <summary>表示するレーダーマップ </summary>
    [NonSerialized] public RaderMap RaderMap;
    private Image _image;

    private void Awake()
    {
        //レーダーテストを検索する
        RaderMap = GameObject.Find("RaderTest").GetComponent<RaderMap>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RaderMap.PanelRock(Enemy);
    }

    /// <summary>
    /// マルチロック時に呼び出される処理
    /// </summary>
    public void MultiLock()
    {
        
    }
}
