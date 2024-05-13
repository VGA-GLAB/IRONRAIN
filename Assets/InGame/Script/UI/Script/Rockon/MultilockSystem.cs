using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultilockSystem : MonoBehaviour
{
    /// <summary>マルチロック中であるか </summary>
    public bool IsMultilock { get; private set; }
    /// <summary>敵のUIリスト </summary>
    public List<EnemyUi> EnemyUis;
    /// <summary>敵のUIリスト（テスト用） </summary>
    public List<AgentScript> AgentScripts;

    
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

    }
}
