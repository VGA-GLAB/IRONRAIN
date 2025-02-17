using System;
using System.Collections.Generic;
using UniRx;
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
    [NonSerialized] public AgentScript _bossAgent; //ボスのアイコン

    private float _initialTotalWidth; //ドローン展開時のTotalWidth
    private Dictionary<Transform, float> _initialXOffsets = new Dictionary<Transform, float>();
    public List<Transform> Funnels = new(); //ファンネルのアイコン

    private RadarMap _radarMap;
    private ReactiveProperty<int> _enemyCountProp = new ReactiveProperty<int>(); //レーダーマップのエネミーリストの要素数
    
    private void Update()
    {
        if (_bossAgent.EnemyIconRectTransform != null)
        {
            BossIconCtrl();
        }
        
        _enemyCountProp.Value = _radarMap.Enemies.Count;

        if (Funnels.Count > 0 && _bossGameObject != null)
        {
            FunnelIconCtrl();
        }
    }

    /// <summary>
    /// ボス戦開始時、初期化を行う
    /// </summary>
    public void BossBattleRadarMapInitialize()
    {
        _radarMap = GetComponent<RadarMap>();
        
        _bossAgent = _bossGameObject.GetComponent<AgentScript>();
        if (_bossAgent == null) Debug.Log("_bossAgentが取得できていません");
        
        // Enemiesの要素数を監視し、変化があったら処理を実行
        _enemyCountProp
            .DistinctUntilChanged()
            .Subscribe(count =>
            {
                if (count == 7)
                {
                    FunnelDeployment();
                }
            })
            .AddTo(this);
        
        // 初期値を設定
        _enemyCountProp.Value = _radarMap.Enemies.Count;
    }

    /// <summary>
    /// ファンネルのアイコンを取得する
    /// </summary>
    private void FunnelDeployment()
    {
        foreach (var enemy in _radarMap.Enemies)
        {
            if (enemy.name != "Boss_8055_Boss_Boss") //ボス以外（ファンネルを取得）
            {
                Funnels.Add(enemy);
            }
        }
        
        int funnelCount = Funnels.Count - 1;
        _initialTotalWidth =  funnelCount * _widthInterval; //初期のTotalWidthを保存しておく
        for (int i = 0; i < Funnels.Count; i++)
        {
            //float xOffset = -_initialTotalWidth / 2 + i * _widthInterval;
            float xOffset = (i - funnelCount / 2.0f) * _widthInterval;
            _initialXOffsets[Funnels[i]] = xOffset;
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
    }

    /// <summary>
    /// ファンネルのアイコンの位置を管理する
    /// </summary>
    private void FunnelIconCtrl()
    {
        for (int i = 0; i < Funnels.Count; i++)
        {
            AgentScript agentScript = Funnels[i].GetComponent<AgentScript>();
            Vector3 funnelDir = Funnels[i].transform.position - _radarMap.PlayerTransform.position;
            funnelDir = Quaternion.Inverse(_radarMap.PlayerTransform.rotation) * funnelDir;
            
            // 各ファンネルの水平位置を中央に対して間隔を持たせて配置
            float xOffset = _initialXOffsets.ContainsKey(Funnels[i]) ? _initialXOffsets[Funnels[i]] : 0f;
            
            agentScript.EnemyIconRectTransform.anchoredPosition3D = new Vector3(
                -(funnelDir.x * _radarMap.Radius + _radarMap.Offset.x + xOffset),
                funnelDir.z * _radarMap.Radius + _radarMap.Offset.y, _radarMap.Offset.z);
        }
    }
}
