﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyUi : MonoBehaviour
{
    public GameObject Enemy;
    /// <summary>表示するレーダーマップ </summary>
    [NonSerialized] public RaderMap RaderMap;
    public GameObject _rockonUi;

    private void Awake()
    {
        //レーダーテストを検索する
        RaderMap = GameObject.FindObjectOfType<RaderMap>();
        //ロックオンUiを非表示にする
        _rockonUi.SetActive(false);
    }

    /// <summary>
    /// 触れたときに呼ばれる処理
    /// </summary>
    public void OnButton()
    {
        RaderMap.PanelRock(Enemy);
    }

    public void Test()
    {
        //Debug.Log("テスト");
    }
}
