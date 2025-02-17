using UnityEngine;

/// <summary>
/// レーダーマップ上に表示される敵アイコンを管理
/// </summary>
public class TargetIcon : MonoBehaviour
{
    [SerializeField] private RadarMap _radarMap;
    private GameObject _enemyIcon;
    private GameObject _lockOnUI;

    public GameObject LockOnUI => _lockOnUI;
    public GameObject EnemyIcon { get; set; }

    private void Awake()
    {
        _lockOnUI = gameObject.transform.GetChild(1).gameObject;
        LockOff();
    }

    public void OnButton()
    {
        _radarMap.LockOn.PanelRock(gameObject);
    }

    /// <summary>ロックオンアイコンを表示する</summary>
    public void LockOn()
    {
        _lockOnUI.SetActive(true);
    }

    /// <summary>ロックオンアイコンを非表示にする</summary>
    public void LockOff()
    {
        _lockOnUI.SetActive(false);
    }
}
