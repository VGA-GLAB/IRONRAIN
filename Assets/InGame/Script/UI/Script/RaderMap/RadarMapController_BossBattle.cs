using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ボス戦中、ボスとファンネルのアイコンを管理する
/// ※テストできていません
/// </summary>
public class RadarMapController_BossBattle : MonoBehaviour
{
    [SerializeField, Header("ボス戦でのレーダー横の倍率")] private float _horizontalMagnification;
    [SerializeField, Header("ボス戦でのレーダー縦の倍率")] private float _verticalMagnification;
    [SerializeField, Header("ボス戦でのファンネルの縦方向の位置補正")] private float _heightLeverage;
    [SerializeField, Header("ボス戦でのファンネルの横間隔の最低値")] private float _widthInterval;
    [SerializeField, Header("正面用のボスUiフラグ")] private bool _isForwardBoss;

    private RadarMap _radarMap;
    private GameObject _bossGameObject;

    private List<GameObject> _fannels = new List<GameObject>();
    private AgentScript[] _agent;
    private AgentScript _bossAgent;

    //ボスのx軸
    private float _bossXPos = 0f;
    private float _bossYPos = 0f;

    private void Start()
    {
        _radarMap = GetComponent<RadarMap>();
    }

    private void Update()
    {
        if(_bossGameObject != null)
        {
            BossIconCtrl();
        }
        
        if (_fannels.Count > 0 && _bossGameObject != null)
        {
            FuunelIconCtrl();
        }
    }

    /// <summary>
    /// ボス戦開始・ファンネル展開時に1回だけ呼びたい
    /// </summary>
    public void BossBattleStart()
    {
        for (int i = 0; i < _radarMap.Enemies.Count; i++)　
        {
            if (_radarMap.Enemies[i].name == "Boss_8055_Boss_Boss") //ボスなら
            {
                _bossAgent = _radarMap.Enemies[i].GetComponent<AgentScript>();
                _bossGameObject = _radarMap.Enemies[i];
                BossIconCtrl();
            }
            else //それ以外＝ファンネルなら
            {
                _agent[i] = _radarMap.Enemies[i].GetComponent<AgentScript>();
                _fannels.Add(_agent[i].gameObject);
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
        _bossAgent.RectTransform.anchoredPosition3D = new Vector3(
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
        //ファンネルの位置を決める
        //ファンネルをボスのx軸から近い順に並べ替える
        var sortedFunnels = _fannels.OrderBy(enemy =>
            Mathf.Abs(enemy.transform.position.x - _bossGameObject.transform.position.x)).ToArray();

        int leftCount = 1;
        int rightCount = 1;

        //真ん中を決める
        for (int i = 0; i < sortedFunnels.Length; i++)
        {
            AgentScript agent = sortedFunnels[i].GetComponent<AgentScript>(); //並べ替えに合わせてリストを更新
            Vector3 enemyDir = sortedFunnels[i].transform.position - _bossGameObject.transform.position; //ボスから敵への方向を計算

            if (i == 0) //x軸を決める
            {
                agent.RectTransform.anchoredPosition3D = new Vector3(
                    _bossXPos, _bossYPos + _heightLeverage, _radarMap.Offset.z); //赤点の位置を決める
            }
            else
            {
                if (enemyDir.x >= 0) //ファンネルがボスよりx軸で正の方向にいたら
                {
                    agent.RectTransform.anchoredPosition3D = new Vector3(
                        _bossXPos - _widthInterval * leftCount,
                        _bossYPos + _heightLeverage, _radarMap.Offset.z);
                    leftCount++;
                }
                else
                {
                    agent.RectTransform.anchoredPosition3D = new Vector3(
                        _bossXPos + _widthInterval * rightCount,
                        _bossYPos + _heightLeverage, _radarMap.Offset.z);
                    rightCount++;
                }
            }
        }
    }
}
