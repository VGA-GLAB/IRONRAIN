using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// レーダーマップのコントローラー
/// </summary>
public class RaderMap : MonoBehaviour
{
    [SerializeField, Header("操作説明パネルのUI")] private CanvasGroup _indicationUI;
    [SerializeField] private Image _indicationIconArea;
    [SerializeField] private IndicationUiScriptableObject _indicationUiSprites;
    [SerializeField, Header("点滅間隔")] private float _indicationUiDuration;
    [SerializeField, Header("点滅時の最低アルファ値")] private float _indicationUiMinAlpha;
    [SerializeField, Header("操作説明パネル拡大にかける時間")] private float _scaleInterval;
    [SerializeField, Header("操作説明パネル縮小にかける時間")] private float _bunshInterval = 0.5f;
    private GameObject _indicationUICanvas; //_indicationUIの子オブジェクトについているオブジェクト
    private Tween _indicationUiTween;

    [SerializeField] private CanvasGroup _miniMapCanvasGroup;
    [SerializeField] private CanvasGroup _purgeUiCanvasGroup;

    [SerializeField, Header("音を鳴らす位置")] private Transform _soundTransform;
    [SerializeField, Header("プレイヤーの位置")] private Transform _player;
    [SerializeField, Header("レーダーの中心のオブジェクト")] private Image _center;
    [SerializeField, Header("レーダーの端までの長さ")] private float _raderLength = 30f;
    [SerializeField, Header("レーダーの半径")] private float _radius = 6f;
    [SerializeField, Header("縮尺")] private float _scaleFactor = 1.5f;
    [SerializeField, Header("X座標の倍率")] private float _horizontalMagnification = 1f;
    [SerializeField, Header("Y座標の倍率")] private float _verticalMagnification = 1f;
    [SerializeField, Header("ロックオン可能距離")] private float _rockonDis = 100f;

    [Tooltip("ボス戦関連")]
    [SerializeField, Header("ボス戦でのUiの位置")] private Vector3 _bossPosition;
    [SerializeField, Header("ボス戦フラグ")] public bool IsBossScene = false;
    [SerializeField, Header("ボス戦でのレーダーの半径")] private float _bossRadius = 0.001f;
    [SerializeField, Header("ボス戦でのレーダーの端までの長さ")] private float _bossRaderLength = 120f;
    [SerializeField, Header("ボス戦でレーダー横の倍率")] private float _bossHorizontalMagnification = 100f;
    [SerializeField, Header("ボス戦でレーダー縦の倍率")] private float _bossVerticalMagnification = 10f;
    [SerializeField, Header("ボス戦でのファンネルの縦方向の位置補正")] private float _hightLeverage = 1f;
    [SerializeField, Header("ボス戦でのファンネルの横間隔の最低値")] private float _widthInterval = 0.1f;
    [SerializeField, Header("正面用のボスUiフラグ")] private bool _isForwardBoss;
    [SerializeField, Tooltip("視野角の基準点")] private Transform _origin;
    [SerializeField, Tooltip("視野角（度数法）")] private float _sightAngle;
    public List<GameObject> Enemies = new();

    /// <summary>ボスオブジェクト</summary>
    private GameObject _bossGameObject;

    private GameObject _nearEnemy;
    private Dictionary<GameObject, Image> _enemyMaps = new();

    /// <summary>Centerからのオフセット</summary>
    private Vector3 _offset;

    /// <summary>現在ロックされているエネミー</summary>
    public GameObject GetRockEnemy { get; private set; }

    /// <summary>一番近い敵 </summary>
    public float GetEnemyDis { get; private set; }

    /// <summary>マルチロック時のエネミー </summary>
    public List<GameObject> MultiLockEnemies { get; } = new();

    /// <summary>タッチ判定</summary>
    private bool _isTouch = false;

    /// <summary>タッチパネルシーケンスに入った時に呼ぶ処理</summary>
    private bool _isStartTouchPanel = false;

    void Start()
    {
        _offset = _center.GetComponent<RectTransform>().anchoredPosition3D;
        _indicationUICanvas = _indicationUI.gameObject.transform.GetChild(0).gameObject;
        _indicationUI.gameObject.SetActive(false);
        _purgeUiCanvasGroup.alpha = 0;
    }

    void Update()
    {
        if (!IsBossScene) //ボス戦以外
        {
            for (int i = 0; i < Enemies.Count; i++)
            {
                AgentScript agent = Enemies[i].GetComponent<AgentScript>();///////

                Vector3 enemyDir = Enemies[i].transform.position;
                //敵の高さとプレイヤーの高さを合わせる
                enemyDir.y = _player.position.y;
                //敵とプレイヤーのベクトルを取ってくる
                enemyDir = Enemies[i].transform.position - _player.position;

                enemyDir = Quaternion.Inverse(_player.rotation) * enemyDir; // ベクトルをプレイヤーに合わせて回転
                enemyDir *= _scaleFactor;
                //赤点の位置を決める
                agent.RectTransform.anchoredPosition3D = new Vector3(
                    enemyDir.x * _radius * _horizontalMagnification + _offset.x,
                    enemyDir.z * _radius * _verticalMagnification + _offset.y, _offset.z);
            }
        }
        else //ボス戦時
        {
            if (!_isForwardBoss) //正面用のボスUiフラグ
            {
                for (int i = 0; i < Enemies.Count; i++)
                {
                    AgentScript agent = Enemies[i].GetComponent<AgentScript>();//////////

                    Vector3 enemyDir = Enemies[i].transform.position;
                    //敵の高さとプレイヤーの高さを合わせる
                    enemyDir.y = _player.position.y;
                    //敵とプレイヤーのベクトルを取ってくる
                    enemyDir = Enemies[i].transform.position - _player.position;

                    enemyDir = Quaternion.Inverse(_player.rotation) * enemyDir; // ベクトルをプレイヤーに合わせて回転
                    enemyDir *= _scaleFactor;
                    //赤点の位置を決める
                    agent.RectTransform.anchoredPosition3D = new Vector3(
                        enemyDir.x * _radius * _bossHorizontalMagnification + _offset.x,
                        enemyDir.z * _radius * _bossVerticalMagnification + _offset.y, _offset.z);
                }
            }
            else
            {
                //ファンネル
                List<GameObject> fannels = new List<GameObject>();
                //ボスのx軸
                float bossXPosition = 0f;
                float bossYPosition = 0f;

                //ここから
                //軸の判定
                string bossDirection = null;

                //ボス戦用のUi処理
                for (int i = 0; i < Enemies.Count; i++)
                {
                    AgentScript agent = Enemies[i].GetComponent<AgentScript>();/////////

                    if (agent.IsBoss)
                    {
                        Vector3 enemyDir = Enemies[i].transform.position;
                        //敵の高さとプレイヤーの高さを合わせる
                        enemyDir.y = _player.position.y;
                        enemyDir = Enemies[i].transform.position - _player.position;
                        Vector3 enemy = Enemies[i].transform.position;
                        //プレイヤーに対するボスの位置を取得

                        // ベクトルをプレイヤーに合わせて回転
                        //赤点の位置を決める
                        enemyDir = Quaternion.Inverse(_player.rotation) * enemyDir;
                        agent.RectTransform.anchoredPosition3D = new Vector3(enemyDir.x * _radius + _offset.x,
                            enemyDir.z * _radius + _offset.y, _offset.z);
                        //ボスの位置を決める
                        bossXPosition = enemyDir.x * _radius + _offset.x;
                        bossYPosition = enemyDir.z * _radius + _offset.y;
                        _bossGameObject = Enemies[i];
                    }
                    else
                    {
                        //ファンネルに追加
                        fannels.Add(agent.gameObject);
                    }
                }

                if (fannels.Count > 0 && _bossGameObject != null)
                {
                    //ファンネルの位置を決める
                    //ファンネルをボスのx軸から近い順に並べ替える
                    var sortedFunnels = fannels.OrderBy(enemy =>
                        Mathf.Abs(enemy.transform.position.x - _bossGameObject.transform.position.x)).ToArray();
                    //敵の総数を計算
                    int totalFunnels = sortedFunnels.Length;

                    float rightLastXPosition = float.MinValue; //右側に最後に入れた物
                    float leftLastXPosition = float.MinValue; //左側で最後に入れた物
                    float centerXPosition = float.MinValue;
                    int leftCount = 1;
                    int rightCount = 1;

                    //真ん中を決める
                    for (int i = 0; i < totalFunnels; i++)
                    {
                        AgentScript agent = sortedFunnels[i].GetComponent<AgentScript>();////////
                        Vector3 enemyDir = sortedFunnels[i].transform.position;
                        // ボスから敵への方向を計算
                        enemyDir = sortedFunnels[i].transform.position - _bossGameObject.transform.position;

                        //y軸を決める
                        float adjustment = 0f;
                        //x軸を決める
                        if (i == 0)
                        {
                            //最後に配置した位置を設定
                            rightLastXPosition = enemyDir.x;
                            leftLastXPosition = enemyDir.x;

                            //赤点の位置を決める
                            agent.RectTransform.anchoredPosition3D = new Vector3(bossXPosition,
                                bossYPosition + _hightLeverage, _offset.z);
                        }
                        else
                        {
                            if (enemyDir.x >= 0)
                            {
                                //赤点の位置を決める
                                agent.RectTransform.anchoredPosition3D = new Vector3(
                                    bossXPosition - _widthInterval * leftCount, bossYPosition + _hightLeverage,
                                    _offset.z);
                                leftCount++;
                            }
                            else
                            {
                                agent.RectTransform.anchoredPosition3D = new Vector3(
                                    bossXPosition + _widthInterval * rightCount, bossYPosition + _hightLeverage,
                                    _offset.z);
                                rightCount++;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// エネミーが生成された時に呼ばれる。
    /// エネミーのアイコンを表示する。
    /// </summary>
    public void GenerateEnemy(GameObject enemy)
    {
        AgentScript agent = enemy.GetComponent<AgentScript>();
        agent.RaderMap = this; //ここいらないかも

        //エネミーのUIを登録
        var enemyUi = Instantiate(agent.Image, _center.transform.parent);
        var uiObj = enemyUi.gameObject.GetComponent<EnemyUi>();
        uiObj.Enemy = enemy;
        if (!_enemyMaps.ContainsKey(enemy)) //enemy オブジェクトが_enemyMapsのキーになかったら
        {
            _enemyMaps.Add(enemy, enemyUi);　//ディクショナリ
            agent.RectTransform = enemyUi.GetComponent<RectTransform>();
            Enemies.Add(enemy);
        }
    }

    /// <summary> 
    /// エネミーが倒された時に呼ばれる。
    /// レーダーマップからアイコンを消す。
    /// </summary>
    public void DestroyEnemy(GameObject enemy)
    {
        if (_enemyMaps.ContainsKey(enemy))
        {
            Destroy(_enemyMaps[enemy].gameObject);
            _enemyMaps.Remove(enemy);
            Enemies.Remove(enemy);
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
            //視野角内にいるのかを判定する
            if (!IsVisible(Enemies[i].gameObject))
                continue;

            float distance = Vector3.Distance(Enemies[i].transform.position, _player.transform.position);
            if (distance < nearDistance)
            {
                nearDistance = distance;
                _nearEnemy = Enemies[i].gameObject;
            }
        }

        return (_nearEnemy, nearDistance);
    }

    /// <summary>ターゲットが視野角内にいるかを判定するメソッド</summary>
    private bool IsVisible(GameObject enemy)
    {
        //自身の向き（正規化したベクトル）
        var selfDir = _origin.forward;
        //ターゲットまでのベクトルと距離
        var targetDir = enemy.transform.position - _origin.position;
        var targetDis = targetDir.magnitude;
        //視野角（の半分）の余弦
        float cosHalfSight = Mathf.Cos(_sightAngle / 2 * Mathf.Deg2Rad);
        // 自身とターゲットへの向きの内積計算
        // ターゲットへの向きベクトルを正規化する必要があることに注意
        float cosTarget = Vector3.Dot(selfDir, targetDir.normalized);

        //視野角の判定
        return cosTarget > cosHalfSight && targetDis < _rockonDis;
    }

    /// <summary> 一番近い敵をロックオンする</summary>
    public void NearEnemyLockOn()
    {
        //タッチパネルロックオンの判定
        if (GetRockEnemy != null)
        {
            var agent = GetRockEnemy.GetComponent<AgentScript>();
            if (!agent.IsDefault && IsVisible(agent.gameObject))
                return;
        }

        //全てのエネミーのロックオンを外す
        ResetUi();
        var nearEnemy = NearEnemy();
        if (nearEnemy.obj is null)
        {
            GetEnemyDis = float.MaxValue;
            GetRockEnemy = null;
            return;
        }

        AgentScript agentScript = nearEnemy.obj.GetComponent<AgentScript>();
        float nearEnemyDis = nearEnemy.Item2;
        agentScript.IsRockon = true;
        //ロックオンUiを表示する
        var rocknUi = _enemyMaps[agentScript.gameObject].gameObject.GetComponent<EnemyUi>();
        rocknUi.LockOnUi.SetActive(true);

        //前のターゲットと違うかを判定
        if (nearEnemy.obj != GetRockEnemy)
        {
            GetRockEnemy = nearEnemy.obj;
            //ターゲットが切り替わる音を出す
            CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Targeting");
        }

        GetEnemyDis = nearEnemyDis;
    }

    /// <summary>Panelを押したときのロックオン処理</summary>
    public void PanelRock(GameObject enemyObject)
    {
        var enemyAgent = enemyObject.GetComponent<AgentScript>();
        if (!enemyAgent.IsDefault)
        {
            //全てのエネミーのロックオンを外す
            ResetUi();
        }
        else
        {
            //視野角内にあるかを判定する
            if (!IsVisible(enemyObject))
                return;

            //全てのエネミーのロックオンを外す
            ResetUi();
            //パネルタッチでのロックオン状態にする
            if (!_enemyMaps.ContainsKey(enemyAgent.gameObject))
                return;
            enemyAgent.IsDefault = false;
            enemyAgent.IsRockon = true;
            //ロックオンUiを表示する
            var rockonUi = _enemyMaps[enemyAgent.gameObject].gameObject.GetComponent<EnemyUi>();
            rockonUi.LockOnUi.SetActive(true);

            GetRockEnemy = enemyAgent.gameObject;
            GetEnemyDis = Vector3.Distance(enemyAgent.gameObject.transform.position, _player.transform.position);

            //ターゲットが切り替わる音を出す
            CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Targeting");
            if (_isStartTouchPanel)
                _isTouch = true;
        }
    }

    /// <summary>すべてのUIをリセットする</summary>
    private void ResetUi()
    {
        //全てのエネミーのロックオンを外す
        foreach (var enemy in Enemies)
        {
            var agent = enemy.GetComponent<AgentScript>();
            agent.IsRockon = false;
            agent.IsDefault = true;
            //エネミーのロックオンUiをすべて非表示にする
            var enemyUi = _enemyMaps[enemy].gameObject.GetComponent<EnemyUi>();
            enemyUi.LockOnUi.SetActive(false);
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
            Gizmos.DrawRay(_origin.transform.position, selfDir * _rockonDis);
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(_origin.transform.position, rightBorder * _rockonDis);
            Gizmos.DrawRay(_origin.transform.position, leftBorder * _rockonDis);
        }
    }


    /// <summary>マルチロックオン処理</summary>
    public void MultiLockOn(List<GameObject> enemies)
    {
        //全てのエネミーのロックオンを外す
        ResetUi();
        if (MultiLockEnemies != null)
            MultiLockEnemies.Clear();

        foreach (var enemy in enemies)
        {
            if (!_enemyMaps.ContainsKey(enemy))
                continue;
            var agentScript = enemy.GetComponent<AgentScript>();
            MultiLockEnemies.Add(enemy);
            agentScript.IsRockon = true;
            //ロックオンされている敵のロックオンUiをすべて表示にする
            var enemyUi = _enemyMaps[enemy].gameObject.GetComponent<EnemyUi>();
            enemyUi.LockOnUi.SetActive(true);
        }
    }

    /// <summary>ボス戦開始時に呼ばれる処理</summary>
    public void BossBattleStart()
    {
        IsBossScene = true;
        _radius = _bossRadius;
        _raderLength = _bossRaderLength;
        _center.rectTransform.localPosition = new Vector3(_bossPosition.x, _bossPosition.y, _bossPosition.z);
        _offset = _center.GetComponent<RectTransform>().anchoredPosition3D;
    }

    public async UniTask WaitTouchPanelAsync(CancellationToken ct)
    {
        _isStartTouchPanel = true;
        await UniTask.WaitUntil(() => _isTouch, cancellationToken: ct);
    }

    /// <summary>
    /// 操作説明パネル（レバーの動かし方などを説明しているパネル）
    /// のアイコンを変更します
    /// </summary>
    public void ChangeIndicationUi(IndicationUiType type)
    {
        if (type == IndicationUiType.None)
        {
            _indicationUiTween.Kill();
            _indicationUiTween = _indicationUICanvas.transform.DOScale(Vector3.zero, _bunshInterval)
                .OnComplete(() => _indicationIconArea.gameObject.SetActive(false));
        }
        else
        {
            _indicationIconArea.gameObject.SetActive(true);
            Sprite sprite = type switch
            {
                IndicationUiType.PushOutsideLever => _indicationUiSprites.PushOutsideLever,
                IndicationUiType.PullOutsideLever => _indicationUiSprites.PullOutsideLever,
                IndicationUiType.ControllerTrigger => _indicationUiSprites.ControllerTrigger,
                IndicationUiType.ControllerWeaponChange => _indicationUiSprites.ControllerWeaponChange,
                IndicationUiType.ControllerMove => _indicationUiSprites.ContorllerMove,
                IndicationUiType.PushThrottle => _indicationUiSprites.PushThrottle,
                IndicationUiType.ThrottleTrigger => _indicationUiSprites.ThrottleTrigger,
                IndicationUiType.Toggle => _indicationUiSprites.Toggle,
                _ => null
            };

            _indicationIconArea.sprite = sprite;
            _indicationIconArea.color = new Color(255, 255, 255, 1);

            //パネルの操作（拡大→UIを点滅させる）
            _indicationUICanvas.transform.localScale = Vector3.zero;
            _indicationUICanvas.transform.DOScale(Vector3.one, _scaleInterval)
                .OnComplete(() => _indicationUiTween = _indicationIconArea.DOFade(_indicationUiMinAlpha, _indicationUiDuration).SetEase(Ease.Flash).SetLoops(-1, LoopType.Yoyo));
        }
    }

    #region 保留
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

/// <summary>
/// 操作説明パネルで表示させたい操作
/// </summary>
public enum IndicationUiType
{
    None,
    PushOutsideLever,
    PullOutsideLever,
    ControllerTrigger,
    ControllerWeaponChange,
    ControllerMove,
    PushThrottle,
    PullThrottle,
    ThrottleTrigger,
    Toggle
}