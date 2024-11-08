using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// レーダーマップに敵のアイコンを表示する
/// </summary>
public class AgentScript : MonoBehaviour
{
    /// <summary>マップに表示するイメージの位置 </summary>
    [NonSerialized] public RectTransform RectTransform;
    /// <summary>表示するレーダーマップ </summary>
    [NonSerialized] public RadarMap RaderMap;
    /// <summary>レーダーマップ上に表示するアイコン</summary>
    [SerializeField] public Image Image;
    /// <summary>ロックオン状態かどうか </summary>
    public bool IsRockon = false;

    [SerializeField, Tooltip("テスト用")] private bool _isTest = false;

    private void Awake()
    {
        RaderMap = FindObjectOfType<RadarMap>(); //レーダーテストを検索する
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
        if (RaderMap != null)
        {
            var enemyIcon = Instantiate(Image, RaderMap._ownIcon.transform.parent);
            var uiObj = enemyIcon.gameObject.GetComponent<TargetIcon>();
            uiObj.Enemy = gameObject;
            if (!RaderMap._enemyMaps.ContainsKey(gameObject)) //自身が_enemyMapsのキーになかったら
            {
                RaderMap._enemyMaps.Add(gameObject, enemyIcon);
                RectTransform = enemyIcon.GetComponent<RectTransform>();
                RaderMap.Enemies.Add(gameObject);
            }
        }
    }
    /// <summary>
    /// エネミーが破棄された時に呼ばれる
    /// </summary>
    public void EnemyIconDestory()
    {
        if (RaderMap != null)
        {
            if (RaderMap._enemyMaps.ContainsKey(gameObject))
            {
                Destroy(RaderMap._enemyMaps[gameObject].gameObject);
                RaderMap._enemyMaps.Remove(gameObject);
                RaderMap.Enemies.Remove(gameObject);
            }
        }
    }
}
