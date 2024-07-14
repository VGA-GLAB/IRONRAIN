using NUnit.Framework.Interfaces;
using System;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AgentScript : MonoBehaviour
{
    /// <summary>マップに表示するイメージの位置 </summary>
    [NonSerialized] public RectTransform RectTransform;
    /// <summary>表示するレーダーマップ </summary>
    [NonSerialized] public RaderMap RaderMap;
    /// <summary> マップに表示するイメージ</summary>
    [SerializeField] public Image Image;
    /// <summary>ロックオン状態 </summary>
    public bool IsRockon = false;
    /// <summary>マップに表示するイメージのデフォルトの色 </summary>
    [SerializeField] public Color _defultColor;
    /// <summary>マップに表示するイメージのロックオン時の色 </summary>
    [SerializeField] public Color _rockonColor;
    /// <summary>マップに表示するイメージのロックオン時の色 </summary>
    public bool IsDefault = true;
    [SerializeField, Tooltip("テスト用")] private bool _isTest = false;

    private void Awake()
    {
        //レーダーテストを検索する
        RaderMap = FindObjectOfType<RaderMap>();
    }

    private void Start()
    {
        if(_isTest)
        {
            EnemyGenerate();
        }  
    }
    /// <summary>
    /// エネミーが生成された時に呼ばれる処理
    /// </summary>
    public void EnemyGenerate()
    {
        if(RaderMap != null)
            RaderMap.GenerateEnemy(this.gameObject);
    }
    /// <summary>
    /// エネミーが破棄された時に呼ばれる
    /// </summary>
    public void EnemyDestory()
    {
        if (RaderMap != null)
            RaderMap.DestroyEnemy(this.gameObject);
    }
}
