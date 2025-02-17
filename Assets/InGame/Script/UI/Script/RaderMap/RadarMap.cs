using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// レーダーマップのコントローラー
/// </summary>
public class RadarMap : MonoBehaviour
{
    public IndicationPanelController _indicationPanelController; //シーケンスの編集が必要になるので仮置き

    [SerializeField] public Transform _playerTransform;
    [SerializeField, Header("自機アイコン")] private Image _ownIcon;
    [SerializeField, Header("レーダーの端までの長さ")] private float _raderLength;
    [SerializeField, Header("レーダーの半径")] private float _radius;
    [SerializeField, Header("縮尺")] private float _scaleFactor;
    [SerializeField, Header("X座標の倍率")] private float _horizontalMagnification;
    [SerializeField, Header("Y座標の倍率")] private float _verticalMagnification;
    private Vector3 _offset;

    [Tooltip("ボス戦関連")]
    [SerializeField, Header("ボス戦でのUIの位置")] private Vector3 _bossPosition;
    [SerializeField, Header("ボス戦でのレーダーの半径")] private float _bossRadius = 0.001f;
    [SerializeField, Header("ボス戦でのレーダーの端までの長さ")] private float _bossRaderLength = 120f;

    public Dictionary<GameObject, Image> _enemyMaps = new();
    public List<Transform> Enemies = new();

    [SerializeField] private LockOn _lockOn;
    private RadarMapController_BossBattle _bossRadarMapCtrl;

    public Transform PlayerTransform { get { return _playerTransform; } } //プレイヤーの位置
    public float Radius { get { return _radius; } } //レーダーの半径
    public Vector3 Offset { get { return _offset; } } //自機のアイコンからの補正

    public Transform OwnIconTranform { get { return _ownIcon.gameObject.transform; } } //自機アイコンの位置
    
    public bool _isTouch = false;
    public bool _isStartTouchPanel = false; //タッチパネルシーケンスに入った時に呼ぶ処理

    public RadarMapController_BossBattle BossRadarMap { get { return _bossRadarMapCtrl; } }
    public LockOn LockOn { get { return _lockOn; } }

    private void Start()
    {
        _offset = _ownIcon.GetComponent<RectTransform>().anchoredPosition3D;
        _bossRadarMapCtrl = GetComponent<RadarMapController_BossBattle>();
        _bossRadarMapCtrl.enabled = false;
    }

    private void Update()
    {
        AgentIconSet(_horizontalMagnification, _verticalMagnification);
    }

    /// <summary>敵アイコンの位置を更新する</summary>
    void AgentIconSet(float horizontalMag, float verticalMag)
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            AgentScript agentScript = Enemies[i].GetComponent<AgentScript>();
            Vector3 enemyDir = Enemies[i].transform.position - _playerTransform.position; //敵と自身のベクトルを求める
            enemyDir = Quaternion.Inverse(_playerTransform.rotation) * enemyDir; // ベクトルをプレイヤーに合わせて回転
            enemyDir *= _scaleFactor; //縮尺に合わせる
            agentScript.EnemyIconRectTransform.anchoredPosition3D = new Vector3(
                enemyDir.x * _radius * horizontalMag + _offset.x,
                enemyDir.z * _radius * verticalMag + _offset.y, _offset.z);
        }
    }

    /// <summary>ボス戦開始時に呼ばれる処理</summary>
    public void BossBattleStart()
    {
        _radius = _bossRadius;
        _raderLength = _bossRaderLength;
        _ownIcon.rectTransform.localPosition = new Vector3(_bossPosition.x, _bossPosition.y, _bossPosition.z);
        _offset = _ownIcon.GetComponent<RectTransform>().anchoredPosition3D;

        //ボス戦用のレーダーマップのクラスを使えるようにする
        _bossRadarMapCtrl.enabled = true;
        _bossRadarMapCtrl.BossBattleRadarMapInitialize();
    }
    
    /// <summary>すべてのUIをリセットする</summary>
    public void ResetUI()
    {
        foreach (var enemy in Enemies)
        {
            var agent = enemy.GetComponent<AgentScript>();
            agent.IsRockOn = false;
            var enemyUi = _enemyMaps[enemy.gameObject].gameObject.GetComponent<TargetIcon>();
            enemyUi.LockOff();
        }
    }

    public async UniTask WaitTouchPanelAsync(CancellationToken ct)
    {
        _isStartTouchPanel = true;
        await UniTask.WaitUntil(() => _isTouch, cancellationToken: ct);
    }
}
