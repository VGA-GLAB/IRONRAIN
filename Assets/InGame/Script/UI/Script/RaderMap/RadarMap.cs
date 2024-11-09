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

    [SerializeField, Header("音を鳴らす位置")] private Transform _soundTransform;
    [SerializeField, Header("プレイヤーの位置")] private Transform _playerTransform;
    [SerializeField, Header("自機アイコン")] private Image _ownIcon;
    [SerializeField, Header("レーダーの端までの長さ")] private float _raderLength;
    [SerializeField, Header("レーダーの半径")] private float _radius;
    [SerializeField, Header("縮尺")] private float _scaleFactor;
    [SerializeField, Header("X座標の倍率")] private float _horizontalMagnification;
    [SerializeField, Header("Y座標の倍率")] private float _verticalMagnification;
    [SerializeField, Header("ロックオン可能距離")] private float _rockonDistance;
    /// <summary>自機のアイコンからの補正</summary>
    private Vector3 _offset;

    [Tooltip("ボス戦関連")]
    [SerializeField, Header("ボス戦フラグ")] public bool IsBossScene = false;
    [SerializeField, Header("ボス戦でのUIの位置")] private Vector3 _bossPosition;
    [SerializeField, Header("ボス戦でのレーダーの半径")] private float _bossRadius = 0.001f;
    [SerializeField, Header("ボス戦でのレーダーの端までの長さ")] private float _bossRaderLength = 120f;
    public List<GameObject> Enemies = new();
    private RadarMapController_BossBattle _bossRadarMapCtrl;

    [SerializeField, Tooltip("視野角の基準点")] private Transform _origin;
    [SerializeField, Tooltip("視野角（度数法）")] private float _sightAngle;

    private GameObject _nearEnemy;
    public Dictionary<GameObject, Image> _enemyMaps = new();

    /// <summary>プレイヤーの位置</summary>
    public Transform PlayerTransform { get { return _playerTransform; } }
    /// <summary>レーダーマップの半径</summary>
    public float Radius { get { return _radius; } }

    /// <summary>自機のアイコンからの補正</summary>
    public Vector3 Offset { get { return _offset; } }

    /// <summary>自機アイコンの位置</summary>
    public Transform OwnIconTranform { get { return _ownIcon.gameObject.transform; } }

    /// <summary>現在ロックされているエネミー</summary>
    public GameObject GetRockEnemy { get; private set; }

    /// <summary>一番近い敵（までの距離...？）</summary>
    public float NearestEnemy { get; private set; }

    /// <summary>マルチロック時のエネミー </summary>
    public List<GameObject> MultiLockEnemies { get; } = new();

    /// <summary>タッチ判定</summary>
    private bool _isTouch = false;

    /// <summary>タッチパネルシーケンスに入った時に呼ぶ処理</summary>
    private bool _isStartTouchPanel = false;


    void Start()
    {
        _offset = _ownIcon.GetComponent<RectTransform>().anchoredPosition3D;
        _bossRadarMapCtrl = GetComponent<RadarMapController_BossBattle>();
    }

    void Update()
    {
        if (!IsBossScene) //ボス戦以外
        {
            AgentAnchorSet(_horizontalMagnification, _verticalMagnification);
        }
    }

    /// <summary>
    /// アイコンのアンカーを、アイコンを表示する場所にセットします
    /// </summary>
    void AgentAnchorSet(float horizontalMag, float verticalMag)
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            AgentScript agent = Enemies[i].GetComponent<AgentScript>(); ////////

            Vector3 enemyDir = Enemies[i].transform.position - _playerTransform.position; //敵と自身のベクトルを求める
            enemyDir = Quaternion.Inverse(_playerTransform.rotation) * enemyDir; // ベクトルをプレイヤーに合わせて回転
            enemyDir *= _scaleFactor; //縮尺に合わせる
            agent.RectTransform.anchoredPosition3D = new Vector3(
                enemyDir.x * _radius * horizontalMag + _offset.x,
                enemyDir.z * _radius * verticalMag + _offset.y, _offset.z);
        }
    }

    #region ロックオン機能

    /// <summary> 一番近い敵をロックオンする</summary>
    public void NearEnemyLockOn()
    {
        //タッチパネルロックオンの判定
        if (GetRockEnemy != null) //ロックオンしていて、ロックオン状態かつ視野角に収まっていたらreturn
        {
            var agent = GetRockEnemy.GetComponent<AgentScript>();
            if (agent.IsRockOn && IsVisible(agent.gameObject))
                return;
        }

        ResetUi();　//全てのエネミーのロックオンを外す

        var nearEnemy = NearEnemy(); //敵がいなかったら

        if (nearEnemy.obj is null)
        {
            NearestEnemy = float.MaxValue;
            GetRockEnemy = null;
            return;
        }

        AgentScript agentScript = nearEnemy.obj.GetComponent<AgentScript>();
        NearestEnemy = nearEnemy.Item2;
        agentScript.IsRockOn = true; //アイコンをロックオン状態にする

        var rocknUi = _enemyMaps[agentScript.gameObject].gameObject.GetComponent<TargetIcon>();
        rocknUi.LockOnUI.SetActive(true);


        if (nearEnemy.obj != GetRockEnemy)　//前のターゲットと違うかを判定
        {
            GetRockEnemy = nearEnemy.obj;
            CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Targeting");　//ターゲットが切り替わる音を出す
        }
    }

    /// <summary>
    /// プレイヤーから１番近い敵のゲームオブジェクトを返す
    /// </summary>
    private (GameObject obj, float) NearEnemy()
    {
        _nearEnemy = null;
        float nearDistance = float.MaxValue;

        //エネミーとの距離を判定する
        for (int i = 0; i < Enemies.Count; i++)
        {
            if (!IsVisible(Enemies[i].gameObject)) continue; //視野角内にいなかったら次

            float distance = Vector3.Distance(Enemies[i].transform.position, _playerTransform.transform.position);
            if (distance < nearDistance)
            {
                nearDistance = distance;
                _nearEnemy = Enemies[i].gameObject;
            }
        }

        return (_nearEnemy, nearDistance);
    }

    /// <summary>ターゲットが視野角内にいるかを判定する</summary>
    private bool IsVisible(GameObject enemy)
    {
        var selfDir = _origin.forward; //自身の向き
        var targetDir = enemy.transform.position - _origin.position; //ターゲットまでのベクトルと距離
        var targetDis = targetDir.magnitude;

        float cosHalfSight = Mathf.Cos(_sightAngle / 2 * Mathf.Deg2Rad);  //視野角（の半分）の余弦
        float cosTarget = Vector3.Dot(selfDir, targetDir.normalized); // 自身とターゲットへの向きの内積計算

        return cosTarget > cosHalfSight && targetDis < _rockonDistance; //視野角の判定
    }

    /// <summary>Panelを押したときのロックオン処理</summary>
    public void PanelRock(GameObject enemyObject)
    {
        var enemyAgent = enemyObject.GetComponent<AgentScript>();

        if (enemyAgent.IsRockOn)
        {
            ResetUi(); //全てのエネミーのロックオンを外す
        }
        else
        {
            if (!IsVisible(enemyObject)) //視野角内にあるかを判定する
                return;

            ResetUi(); //全てのエネミーのロックオンを外す

            if (!_enemyMaps.ContainsKey(enemyAgent.gameObject))
                return;

            enemyAgent.IsRockOn = true;

            var rockonUi = _enemyMaps[enemyAgent.gameObject].gameObject.GetComponent<TargetIcon>();
            rockonUi.LockOn();

            GetRockEnemy = enemyAgent.gameObject;
            NearestEnemy = Vector3.Distance(enemyAgent.gameObject.transform.position, _playerTransform.transform.position);

            CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Targeting");　//ターゲットが切り替わる音を出す
            if (_isStartTouchPanel)
                _isTouch = true;
        }
    }

    /// <summary>すべてのUIをリセットする</summary>
    private void ResetUi()
    {
        foreach (var enemy in Enemies)
        {
            var agent = enemy.GetComponent<AgentScript>();
            agent.IsRockOn = false;
            var enemyUi = _enemyMaps[enemy].gameObject.GetComponent<TargetIcon>();
            enemyUi.LockOff();
        }
    }

    //視野角をギズモ化
    private void OnDrawGizmos()
    {
        // 視界の範囲（正面及び左右の端）をギズモとして描く
        if (_origin != null)
        {
            Vector3 selfDir = _origin.forward;
            Vector3 rightBorder = Quaternion.Euler(0, _sightAngle / 2, 0) * selfDir; //右端
            Vector3 leftBorder = Quaternion.Euler(0, -1 * _sightAngle / 2, 0) * selfDir; //左端
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_origin.transform.position, selfDir * _rockonDistance);
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(_origin.transform.position, rightBorder * _rockonDistance);
            Gizmos.DrawRay(_origin.transform.position, leftBorder * _rockonDistance);
        }
    }


    /// <summary>マルチロックオン処理</summary>
    public void MultiLockOn(List<GameObject> enemies)
    {
        ResetUi(); //全てのエネミーのロックオンを外す

        if (MultiLockEnemies != null)
            MultiLockEnemies.Clear();

        foreach (var enemy in enemies)
        {
            if (!_enemyMaps.ContainsKey(enemy)) continue;

            var agentScript = enemy.GetComponent<AgentScript>();
            MultiLockEnemies.Add(enemy);
            agentScript.IsRockOn = true;

            var enemyUi = _enemyMaps[enemy].gameObject.GetComponent<TargetIcon>();
            enemyUi.LockOn();
        }
    }
    #endregion

    /// <summary>ボス戦開始時に呼ばれる処理</summary>
    public void BossBattleStart()
    {
        IsBossScene = true;
        _radius = _bossRadius;
        _raderLength = _bossRaderLength;
        _ownIcon.rectTransform.localPosition = new Vector3(_bossPosition.x, _bossPosition.y, _bossPosition.z);
        _offset = _ownIcon.GetComponent<RectTransform>().anchoredPosition3D;
        _bossRadarMapCtrl.BossBattleStart();
    }

    public async UniTask WaitTouchPanelAsync(CancellationToken ct)
    {
        _isStartTouchPanel = true;
        await UniTask.WaitUntil(() => _isTouch, cancellationToken: ct);
    }

    #region 消したとき「SequenceSystem」でエラーが出るので一旦そのままにしておきます
    /// <summary>パージシーケンスが始まる時に呼ぶ処理</summary>
    public void StartPurgeSequence()
    {
        ////パネルタッチ音を鳴らす
        //CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Panel_Tap");
        ////ミニマップを非表示にする
        //_miniMapCanvasGroup.alpha = 0;
        ////パージUiを表示する
        //_purgeUiCanvasGroup.alpha = 1;
    }


    public void EndPurgeSequence()
    {
        ////パネルタッチ音を鳴らす
        //CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Panel_Tap");
        ////パージUiを非表示する
        //_purgeUiCanvasGroup.alpha = 0;
        ////ミニマップを非表示にする
        //_miniMapCanvasGroup.alpha = 1;
    }
    #endregion
}
