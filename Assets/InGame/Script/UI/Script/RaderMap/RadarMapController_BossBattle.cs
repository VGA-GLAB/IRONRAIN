using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ボス戦中、ボスとファンネルのアイコンを管理する
/// </summary>
public class RadarMapController_BossBattle : MonoBehaviour
{

    [SerializeField, Header("ボス戦でのレーダー横の倍率")] private float _horizontalMagnification;
    [SerializeField, Header("ボス戦でのレーダー縦の倍率")] private float _verticalMagnification;
    [SerializeField, Header("ボス戦でのファンネルの縦方向の位置補正")] private float _heightLeverage;
    [SerializeField, Header("ボス戦でのファンネルの横間隔の最低値")] private float _widthInterval;
    [SerializeField, Header("正面用のボスUIフラグ")] private bool _isForwardBoss;
    
    [SerializeField] private GameObject _bossGameObject;
    public AgentScript _bossAgent; //ボスのアイコン
    public List<AgentScript> _fannels = new(); //ファンネルのアイコン

    private RadarMap _radarMap;
    
    //ボスのx軸
    private float _bossXPos = 0f;
    private float _bossYPos = 0f;

    private void Start()
    {
        _radarMap = GetComponent<RadarMap>();
    }

    private void Update()
    {
        if(_bossAgent.EnemyIconRectTransform != null)
        {
            BossIconCtrl();
        }
        
        if (_fannels.Count > 0 && _bossGameObject != null)
        {
            FuunelIconCtrl();
        }
    }

    /// <summary>
    /// ボス戦開始
    /// </summary>
    public void BossBattleStart()
    {
        _bossAgent = _bossGameObject.GetComponent<AgentScript>();
        if (_bossAgent == null) Debug.Log("_bossAgentが取得できていません");
    }

    [ContextMenu("FunnelDeployment")]
    /// <summary>
    /// ファンネル展開時に1回だけ呼びたい
    /// </summary>
    public void FuunelDeployment()
    {
        for (int i = 0; i < _radarMap.Enemies.Count; i++)
        {
            if (_radarMap.Enemies[i].name != "Boss_8055_Boss_Boss") //ボス以外（ファンネルを取得）
            {
                _fannels.Add(_radarMap.Enemies[i]);
            }
        }
    }

    /// <summary>
    /// ボスのアイコンの位置を管理する
    /// </summary>
    private void BossIconCtrl()
    {
        Vector3 bossDir = _bossGameObject.transform.position - _radarMap.PlayerTransform.position;
        bossDir = Quaternion.Inverse(_radarMap.PlayerTransform.rotation) * bossDir; // ベクトルをプレイヤーに合わせて回転
        _bossAgent.EnemyIconRectTransform.anchoredPosition3D = new Vector3(
            bossDir.x * _radarMap.Radius + _radarMap.Offset.x,
            bossDir.z * _radarMap.Radius + _radarMap.Offset.y, _radarMap.Offset.z); //アンカーをセット

        _bossXPos = bossDir.x * _radarMap.Radius + _radarMap.Offset.x;
        _bossYPos = bossDir.z * _radarMap.Radius + _radarMap.Offset.y;
    }

    /// <summary>
    /// ファンネルのアイコンの位置を管理する
    /// </summary>
    private void FuunelIconCtrl()
    {

        float totalWidth = (_fannels.Count - 1) * _widthInterval;

        for (int i = 0; i < _fannels.Count; i++)
        {
            Vector3 funnelDir = _fannels[i].transform.position - _radarMap.PlayerTransform.position;
            funnelDir = Quaternion.Inverse(_radarMap.PlayerTransform.rotation) * funnelDir;

            // 各ファンネルの水平位置を中央に対して間隔を持たせて配置
            float xOffset = -totalWidth / 2 + i * _widthInterval;

            _fannels[i].EnemyIconRectTransform.anchoredPosition3D = new Vector3(
                funnelDir.x * _radarMap.Radius + _radarMap.Offset.x + xOffset,
                funnelDir.z * _radarMap.Radius + _radarMap.Offset.y, _radarMap.Offset.z);
        }
    }
}
