using System;
using UnityEngine;

/// <summary>
/// レーダーマップ上に表示される敵アイコンを管理
/// </summary>
public class TargetIcon : MonoBehaviour
{
    public GameObject Enemy;
    /// <summary>表示するレーダーマップ </summary>
    [NonSerialized] public RadarMap RaderMap;
    [NonSerialized] public GameObject LockOnUI;


    private void Awake()
    {
        RaderMap = FindObjectOfType<RadarMap>(); //レーダーテストを検索する
        LockOnUI = gameObject.transform.GetChild(1).gameObject;
        LockOff();
    }

    /// <summary>
    /// 触れたときに呼ばれる処理
    /// </summary>
    public void OnButton()
    {
        RaderMap.PanelRock(Enemy);
    }

    /// <summary>ロックオンアイコンを表示する</summary>
    public void LockOn()
    {
        LockOnUI.SetActive(true);
    }

    /// <summary>ロックオンアイコンを非表示にする</summary>
    public void LockOff()
    {
        LockOnUI.SetActive(false);
    }
}
