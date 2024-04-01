using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSwitch : MonoBehaviour
{
    [SerializeField, Tooltip("詳細パネル")] GameObject[] _panel;
    [SerializeField, Tooltip("パネルの状態")] PanelStatus _status;
    /// <summary>
    /// パネルの状態
    /// </summary>
    public enum PanelStatus
    {
        detail,
        map,
    }

    void Start()
    {
        //初期設定で詳細パネルを表示する
        _status = PanelStatus.detail;
        SwitchPanel(0);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(_status == PanelStatus.map)
            {
                SwitchPanel(0);
                _status = PanelStatus.detail;
            }  
            else if(_status == PanelStatus.detail)
            {
                SwitchPanel(1);
                _status = PanelStatus.map;
            }
                
        }
    }

    /// <summary>
    /// パネルを切り替えるメソッド
    /// </summary>
    /// <param name="index">表示させたいパネルの添え字</param>
    void SwitchPanel(int  index)
    {
        for(int i = 0;  i < _panel.Length; i++)
        {
            if(i  == index)
                _panel[i].SetActive(true);
            else
                _panel[i].SetActive(false);
        }
    }
}
