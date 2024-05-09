using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyUi : MonoBehaviour, IPointerDownHandler
{
    public GameObject Enemy;
    /// <summary>表示するレーダーマップ </summary>
    [NonSerialized] public RaderMap RaderMap;

    private void Awake()
    {
        //レーダーテストを検索する
        RaderMap = GameObject.Find("RaderTest").GetComponent<RaderMap>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        RaderMap.PanelRock(Enemy);
    }
}
