using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// レーダーマップに敵のアイコンを表示する
/// </summary>
public class AgentScript : MonoBehaviour
{
    /// <summary>マップに表示するイメージの位置 </summary>
    [NonSerialized] public RectTransform EnemyIconRectTransform;

    [NonSerialized] public RadarMap RadarMap;
    [SerializeField] public Image Icon;

    [SerializeField, Tooltip("テスト用")] private bool _isTest = false;

    public bool IsRockOn { get; set; }

    private void Awake()
    {
        RadarMap = FindAnyObjectByType<RadarMap>(); //レーダーテストを検索する
    }

    private void Start()
    {
        if (_isTest)
        {
            EnemyIconInst();
        }
    }
    /// <summary>
    /// エネミーが生成された時に呼ばれる
    /// レーダーマップの子オブジェクトにアイコンを生成する
    /// </summary>
    public void EnemyIconInst()
    {
        if (RadarMap != null)
        {
            var enemyIcon = Instantiate(Icon, RadarMap.OwnIconTranform.parent);
            EnemyIconRectTransform = enemyIcon.gameObject.GetComponent<RectTransform>();
            var uiObj = enemyIcon.gameObject.GetComponent<TargetIcon>();
            uiObj.EnemyIcon = gameObject;

            
            if (!RadarMap._enemyMaps.ContainsKey(gameObject)) //自身が_enemyMapsのキーになかったら
            {
                RadarMap._enemyMaps.Add(gameObject, enemyIcon);
                EnemyIconRectTransform = enemyIcon.GetComponent<RectTransform>();
                RadarMap.Enemies.Add(gameObject.GetComponent<AgentScript>());
            }
            
        }
    }
    /// <summary>
    /// エネミーが破棄された時に呼ばれる
    /// </summary>
    public void EnemyIconDestory()
    {
        if (RadarMap != null)
        {
            if (RadarMap._enemyMaps.ContainsKey(gameObject))
            {
                Destroy(RadarMap._enemyMaps[gameObject].gameObject);
                RadarMap._enemyMaps.Remove(gameObject);
                RadarMap.Enemies.Remove(gameObject.GetComponent<AgentScript>());
                RadarMap.BossRadarMap._funnels.Remove(gameObject.GetComponent<AgentScript>());
            }
        }
    }
}
