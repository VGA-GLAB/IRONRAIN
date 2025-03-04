﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum IndicationUiType
{
    None,
    PushOutsideLever,
    PullOutsideLever,
    ControllerTrigger,
    ControllerWeaponChange,
    ContorllerMove,
    PushThrottle,
    PullThrottle,
    ThrottleTrigger,
    Toggle
}
public class RaderMap : MonoBehaviour
{
    [Header("IndicationUi")]
    [SerializeField] private GameObject _indicationUi;
    [Header("IndicationUiCanvas")]
    [SerializeField] private GameObject _indicationUiCanvas;
    [Header("LeverUi")]
    [SerializeField] private Image _leverUi;
    [Header("ContrllerUi")]
    [SerializeField] private Image _contrllerUi;
    [Header("ThrottleUi")]
    [SerializeField] private Image _throttleUi;
    [Header("IndicationScriptableObject")]
    [SerializeField] private IndicationUiScriptableObject _indicationUiScriptableObject;
    private Coroutine _blinkCoroutine;
    [Header("点滅間隔")]
    [SerializeField] private float _indicationUiDuration = 1f;
    [Header("点滅時の最低アルファ値")]
    [SerializeField] private float _indicationUiMinAlpha = 0.2f;
    [Header("大きくなるまでの時間")]
    [SerializeField] private float _scaleInterval = 0.5f;
    [Header("消えるまでの時間")]
    [SerializeField] private float _bunshInterval = 0.5f;

    [Header("MiniMapのCanvasGroup")]
    [SerializeField] private CanvasGroup _miniMapCanvasGroup;
    [Header("PurgeUiのCanvasGroup")]
    [SerializeField] private CanvasGroup _purgeUiCanvasGroup;
    /// <summary0>
    /// 敵のリスト
    /// </summary>
    [FormerlySerializedAs("_enemies")] public List<GameObject> Enemies = new List<GameObject>();
    /// <summary>
    /// 敵UIのリスト(GameObject:実際の敵、Image:Ui)
    /// </summary>
    public Dictionary<GameObject, Image> EnemyMaps = new Dictionary<GameObject, Image>();
    [Header("音を鳴らす位置")]
    [SerializeField] private Transform _soundTransform;
    [Header("プレイヤーの位置")]
    [SerializeField, Tooltip("プレイヤーの位置")] private Transform _player;
    [Header("レーダーの中心のオブジェクト")]
    [SerializeField, Tooltip("UIの真ん中")] private Image _center;
    [Header("レーダーの端までの長さ")]
    [SerializeField, Tooltip("レーダーの大きさ")] private float _raderLength = 30f;
    [Header("レーダーの半径")]
    [SerializeField, Tooltip("半径")] private float _radius = 6f;
    [Header("縮尺")]
    [SerializeField] private float _scaleFactor = 1.5f;
    [Header("X座標の倍率")]
    [SerializeField] private float _horizontalMagnification = 1f;
    [Header("Y座標の倍率")]
    [SerializeField] private float _verticalMagnification = 1f;
    [Header("ロックオン可能距離")]
    [SerializeField, Tooltip("ロックオン可能距離")] private float _rockonDis = 100f;

    [Header("ボス戦")] [Header("ボス戦でのUiの位置")] [SerializeField]
    private Vector3 _bossPosition;
    [Header("ボス戦フラグ")]
    [SerializeField, Tooltip("ボス戦フラグ")] public bool IsBossScene = false;
    [Header("ボス戦でのレーダーの半径")]
    [SerializeField] private float _bossRadius = 0.001f;
    [Header("ボス戦でのレーダーの端までの長さ")]
    [SerializeField] private float _bossRaderLength = 120f;

    [Header("ボス戦でレーダー横の倍率")]
    [SerializeField] private float _bossHorizontalMagnification = 100f;
    [Header("ボス戦でレーダー縦の倍率")]
    [SerializeField] private float _bossVerticalMagnification = 10f;
    [Header("ボス戦でのファンネルの縦方向の位置補正")]
    [SerializeField] private float _hightLeverage = 1f;
    [Header("ボス戦でのファンネルの横間隔の最低値")]
    [SerializeField] private float _widthInterval = 0.1f;
    //[Header("ボス戦でのファンネルの縦方向の位置補正")]
    ///SerializeField] private float _hight = 0.1f;
    ///
    [Header("正面用のボスUiフラグ")]
    [SerializeField] private bool _isForwardBoss;
    
    /// <summary>
    /// ボスオブジェクト
    /// </summary>
    private GameObject _bossGameObject = default;
    /// <summary>Centerからのオフセット</summary>
    private Vector3 _offset;
    /// <summary>現在ロックされているエネミー</summary>
    private GameObject _nowRockEnemy;
    public GameObject GetRockEnemy
    {
        get { return _nowRockEnemy; }
    }
    /// <summary>一番近い敵 </summary>
    private float _enemyDistance;
    public float GetEnemyDis
    {
        get { return _enemyDistance; }
    }

    /// <summary>マルチロック時のエネミー </summary>
    private List<GameObject> _multiLockEnemys = new List<GameObject>();

    public List<GameObject> MultiLockEnemys
    {
        get { return _multiLockEnemys; }
    }

    /// <summary>
    /// タッチ判定
    /// </summary>
    private bool _isTouch = false;

    /// <summary>
    /// タッチパネルシーケンスに入った時に呼ぶ処理
    /// </summary>
    private bool _isStartTouchPanel = false;

    
    // Start is called before the first frame update
    void Start()
    {
        CanvasGroup canvasGrop = _indicationUi.GetComponent<CanvasGroup>();
        canvasGrop.alpha = 0;
        _offset = _center.GetComponent<RectTransform>().anchoredPosition3D;
        _purgeUiCanvasGroup.alpha = 0;
        //_mouseMultilockSystem = GameObject.FindObjectOfType<MouseMultilockSystem>();
        //_pokeInteractionBase = FindObjectOfType<UiPokeInteraction>();
    }

    void Update()
    {
        //テスト用
        //if (Input.GetKeyDown(KeyCode.C))
        //    IconTest();

        //if (Input.GetKeyDown(KeyCode.B))
        //    IconDeleteTest();

        if (!IsBossScene)
        {
            for (int i = 0; i < Enemies.Count; i++)
            {
                AgentScript agent = Enemies[i].GetComponent<AgentScript>();
        
                Vector3 enemyDir = Enemies[i].transform.position;
                //敵の高さとプレイヤーの高さを合わせる
                enemyDir.y = _player.position.y;
                //敵とプレイヤーのベクトルを取ってくる
                enemyDir = Enemies[i].transform.position - _player.position;
        
                enemyDir = Quaternion.Inverse(_player.rotation) * enemyDir; // ベクトルをプレイヤーに合わせて回転
                enemyDir *= _scaleFactor;
                //enemyDir = Vector3.ClampMagnitude(enemyDir, _raderLength); // ベクトルの長さを制限
                //enemyDir.x = Mathf.Clamp(enemyDir.x * _scaleFactor, -_raderLength, _raderLength); // ベクトルの長さを制限
                //enemyDir.z = Mathf.Clamp(enemyDir.z * _scaleFactor, -_raderLength, _raderLength);
                //赤点の位置を決める
                agent.RectTransform.anchoredPosition3D = new Vector3(enemyDir.x * _radius * _horizontalMagnification + _offset.x, enemyDir.z * _radius * _verticalMagnification + _offset.y, _offset.z);
            }
        }
        else
        {
            if(!_isForwardBoss)
            {
                for (int i = 0; i < Enemies.Count; i++)
                {
                    AgentScript agent = Enemies[i].GetComponent<AgentScript>();

                    Vector3 enemyDir = Enemies[i].transform.position;
                    //敵の高さとプレイヤーの高さを合わせる
                    enemyDir.y = _player.position.y;
                    //敵とプレイヤーのベクトルを取ってくる
                    enemyDir = Enemies[i].transform.position - _player.position;

                    enemyDir = Quaternion.Inverse(_player.rotation) * enemyDir; // ベクトルをプレイヤーに合わせて回転
                    enemyDir *= _scaleFactor;
                    //赤点の位置を決める
                    agent.RectTransform.anchoredPosition3D = new Vector3(enemyDir.x * _radius * _bossHorizontalMagnification + _offset.x, enemyDir.z * _radius * _bossVerticalMagnification + _offset.y, _offset.z);
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
                    AgentScript agent = Enemies[i].GetComponent<AgentScript>();


                    if (agent.IsBoss)
                    {
                        Vector3 enemyDir = Enemies[i].transform.position;
                        //敵の高さとプレイヤーの高さを合わせる
                        enemyDir.y = _player.position.y;
                        enemyDir = Enemies[i].transform.position - _player.position;
                        Vector3 enemy = Enemies[i].transform.position;
                        //プレイヤーに対するボスの位置を取得

                        enemyDir = Quaternion.Inverse(_player.rotation) * enemyDir; // ベクトルをプレイヤーに合わせて回転
                                                                                    //赤点の位置を決める
                        agent.RectTransform.anchoredPosition3D = new Vector3(enemyDir.x * _radius + _offset.x, enemyDir.z * _radius + _offset.y, _offset.z);
                        bossXPosition = enemyDir.x * _radius + _offset.x;
                        bossYPosition = enemyDir.z * _radius + _offset.y;
                        _bossGameObject = Enemies[i];

                    } //ボスの位置を決める
                    else
                    {
                        fannels.Add(agent.gameObject);
                    }//ファンネルに追加
                }

                if(fannels.Count > 0 && _bossGameObject != null)
                {
                    //ファンネルの位置を決める
                    //ファンネルをボスのx軸から近い順に並べ替える
                    var sortedFunnels = fannels.OrderBy(enemy => Mathf.Abs(enemy.transform.position.x - _bossGameObject.transform.position.x)).ToArray();
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
                        AgentScript agent = sortedFunnels[i].GetComponent<AgentScript>();
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
                            agent.RectTransform.anchoredPosition3D = new Vector3(bossXPosition, bossYPosition + _hightLeverage, _offset.z);
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
                                //enemyDir.x = leftLastXPosition - _widthInterval; // 最後の位置を更新
                                //leftLastXPosition -= _widthInterval;
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
    /// エネミーが生成された時に呼ぶメソッド
    /// </summary>
    /// <param name="enemy">エネミーオブジェクト</param>
    public void GenerateEnemy(GameObject enemy)
    {
        AgentScript agent = enemy.GetComponent<AgentScript>();
        agent.RaderMap = this;
        //エネミーのUIを登録
        var enemyUi = Instantiate(agent.Image, _center.transform.parent);
        var uiObj = enemyUi.gameObject.GetComponent<EnemyUi>();
        uiObj.Enemy = enemy;
        if (!EnemyMaps.ContainsKey(enemy))
        {
            EnemyMaps.Add(enemy, enemyUi);
            agent.RectTransform = enemyUi.GetComponent<RectTransform>();
            Enemies.Add(enemy);
            //_pokeInteractionBase.AddIcon(enemyUi.gameObject);
        }
    }

    /// <summary>
    /// エネミーが倒された時に呼ばれるメソッド
    /// </summary>
    /// <param name="enemy"></param>
    public void DestroyEnemy(GameObject enemy)
    {
        if (EnemyMaps.ContainsKey(enemy))
        {
            //_pokeInteractionBase.RemoveIcon(EnemyMaps[enemy].gameObject);
            Destroy(EnemyMaps[enemy].gameObject);
            EnemyMaps.Remove(enemy);
            Enemies.Remove(enemy);
        }
    }

    /// <summary>
    /// 一番近い敵のゲームオブジェクト
    /// </summary>
    private GameObject _nearEnemy;

    /// <summary>
    /// プレイヤーから１番近い敵のゲームオブジェクトを返すメソッド
    /// </summary>
    /// <returns>最も近い敵を返す</returns>
    private (GameObject obj,float) NearEnemy()
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

    [SerializeField, Tooltip("視野角の基準点")] private Transform _origin;
    [SerializeField, Tooltip("視野角（度数法）")] private float _sightAngle;
    /// <summary>
    /// ターゲットが視野角内にいるかを判定するメソッド
    /// </summary>
    /// <param name="enemy">検索したい敵</param>
    /// <returns></returns>
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

    /// <summary>
    /// 一番近い敵をロックオンする
    /// </summary>
    public void NearEnemyLockon()
    {
        //タッチパネルロックオンの判定
        if(_nowRockEnemy != null)
        {
            var agent = _nowRockEnemy.GetComponent<AgentScript>();
            if (!agent.IsDefault && IsVisible(agent.gameObject))
                return;
        }

        //全てのエネミーのロックオンを外す
        ResetUi();
        var nearEnemy = NearEnemy();
        if(nearEnemy.obj is null)
        {
            _enemyDistance = float.MaxValue;
            _nowRockEnemy = null;
            return;
        }

        AgentScript agentScript = nearEnemy.obj.GetComponent<AgentScript>();
        float nearEnemyDis = nearEnemy.Item2;
        agentScript.IsRockon = true;
        //EnemyMaps[agentScript.gameObject].color = agentScript._rockonColor;
        //ロックオンUiを表示する
        var rockonUi = EnemyMaps[agentScript.gameObject].gameObject.GetComponent<EnemyUi>();
        rockonUi.LockOnUi.SetActive(true);

        //前のターゲットと違うかを判定
        if (nearEnemy.obj != _nowRockEnemy)
        {
            _nowRockEnemy = nearEnemy.obj;
            //ターゲットが切り替わる音を出す
            CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Targeting");
        }
        _enemyDistance = nearEnemyDis;
    }

    /// <summary>
    /// Panelを押したときのロックオン処理
    /// </summary>
    /// <param name="enemyObject"></param>
    public void PanelRock(GameObject enemyObject)
    {
        //var ui = enemyUi.GetComponent<EnemyUi>();
        //var enemyObj = enemyObject.gameObject;
        var enemyAgent = enemyObject.GetComponent<AgentScript>();
        if(!enemyAgent.IsDefault)
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
            if (!EnemyMaps.ContainsKey(enemyAgent.gameObject))
                return;
            enemyAgent.IsDefault = false;
            enemyAgent.IsRockon = true;
            //EnemyMaps[enemyAgent.gameObject].color = enemyAgent._rockonColor;
            //ロックオンUiを表示する
            var rockonUi = EnemyMaps[enemyAgent.gameObject].gameObject.GetComponent<EnemyUi>();
            rockonUi.LockOnUi.SetActive(true);

            _nowRockEnemy = enemyAgent.gameObject;
            _enemyDistance = Vector3.Distance(enemyAgent.gameObject.transform.position, _player.transform.position);
            
            //ターゲットが切り替わる音を出す
            CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Targeting");
            if(_isStartTouchPanel)
                _isTouch = true;
        }
    }

    /// <summary>
    /// すべてのUIをリセットする
    /// </summary>
    private void ResetUi()
    {
        //全てのエネミーのロックオンを外す
        foreach (var enemy in Enemies)
        {
            var agent = enemy.GetComponent<AgentScript>();
            agent.IsRockon = false;
            agent.IsDefault = true;
            //EnemyMaps[enemy].color = agent._defultColor;
            //エネミーのロックオンUiをすべて非表示にする
            var enemyUi = EnemyMaps[enemy].gameObject.GetComponent<EnemyUi>();
            enemyUi.LockOnUi.SetActive(false);
        }
    }
    //視野角をギズモ化
    private void OnDrawGizmos()
    {
        // 視界の範囲（正面及び左右の端）をギズモとして描く
        if(_origin != null)
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


    // <summary>
    // マルチロックオン処理
    // </summary>
    public void MultiLockon(List<GameObject> enemys)
    {
        //全てのエネミーのロックオンを外す
        ResetUi();
        if(_multiLockEnemys != null)
            _multiLockEnemys.Clear();

        foreach (var enemy in enemys)
        {
            if (!EnemyMaps.ContainsKey(enemy))
                continue;
            var agentScript = enemy.GetComponent<AgentScript>();
            _multiLockEnemys.Add(enemy);
            agentScript.IsRockon = true;
            //EnemyMaps[agentScript.gameObject].color = agentScript._rockonColor;
            //ロックオンされている敵のロックオンUiをすべて表示にする
            var enemyUi = EnemyMaps[enemy].gameObject.GetComponent<EnemyUi>();
            enemyUi.LockOnUi.SetActive(true);
        }
    }

    /// <summary>
    /// ボス戦開始時に呼ばれる処理
    /// </summary>
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

        //Debug.Log("通った");
    }

    /// <summary>
    /// パージシーケンスが始まる時に呼ぶ処理
    /// </summary>
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

    Tweener tweener;
    /// <summary>
    /// チュートリアルアイコンの変更
    /// </summary>
    /// <param name="type">変えたいアイコン</param>
    public void ChangeIndicationUi(IndicationUiType type)
    {
        StopBlinking();
        CanvasGroup canvasGrop = _indicationUi.GetComponent<CanvasGroup>();
        switch (type)
        {
            case IndicationUiType.None:
                tweener.Kill();
                tweener = _indicationUiCanvas.transform.DOScale(Vector3.zero, _bunshInterval).SetAutoKill(true) // 0.5秒でスケール1にする
                   .OnComplete(() => canvasGrop.alpha = 0);    // スケールダウン完了後にアルファ値を0にする
                break;
            case IndicationUiType.PushOutsideLever:
                canvasGrop.alpha = 1;
                _leverUi.gameObject.SetActive(true);
                _contrllerUi.gameObject.SetActive(false);
                _throttleUi.gameObject.SetActive(false);
                _leverUi.sprite = _indicationUiScriptableObject.PushOutsideLever;
                // スケールを0から1にアニメーションさせ、完了したら点滅を開始
                _indicationUiCanvas.transform.localScale = Vector3.zero; // 初期スケールを0に設定
                tweener = _indicationUiCanvas.transform.DOScale(Vector3.one, _scaleInterval).SetAutoKill(true) // 0.5秒でスケール1にする
                    .OnComplete(() => StartBlinking(_leverUi));    // スケールアップ完了後に点滅開始
                break;
            case IndicationUiType.PullOutsideLever:
                canvasGrop.alpha = 1;
                _leverUi.gameObject.SetActive(true);
                _contrllerUi.gameObject.SetActive(false);
                _throttleUi.gameObject.SetActive(false);
                _leverUi.sprite = _indicationUiScriptableObject.PullOutsideLever;
                // スケールを0から1にアニメーションさせ、完了したら点滅を開始
                _indicationUiCanvas.transform.localScale = Vector3.zero; // 初期スケールを0に設定
                tweener = _indicationUiCanvas.transform.DOScale(Vector3.one, _scaleInterval).SetAutoKill(true) // 0.5秒でスケール1にする
                    .OnComplete(() => StartBlinking(_leverUi));    // スケールアップ完了後に点滅開始
                break;
            case IndicationUiType.ControllerTrigger:
                canvasGrop.alpha = 1;
                _leverUi.gameObject.SetActive(false);
                _contrllerUi.gameObject.SetActive(true);
                _throttleUi.gameObject.SetActive(false);
                _contrllerUi.sprite = _indicationUiScriptableObject.ControllerTrigger;
                // スケールを0から1にアニメーションさせ、完了したら点滅を開始
                _indicationUiCanvas.transform.localScale = Vector3.zero; // 初期スケールを0に設定
                tweener = _indicationUiCanvas.transform.DOScale(Vector3.one, _scaleInterval).SetAutoKill(true) // 0.5秒でスケール1にする
                    .OnComplete(() => StartBlinking(_contrllerUi));    // スケールアップ完了後に点滅開始
                break;
            case IndicationUiType.ControllerWeaponChange:
                canvasGrop.alpha = 1;
                _leverUi.gameObject.SetActive(false);
                _contrllerUi.gameObject.SetActive(true);
                _throttleUi.gameObject.SetActive(false);
                _contrllerUi.sprite = _indicationUiScriptableObject.ControllerWeaponChange;
                // スケールを0から1にアニメーションさせ、完了したら点滅を開始
                _indicationUiCanvas.transform.localScale = Vector3.zero; // 初期スケールを0に設定
                tweener = _indicationUiCanvas.transform.DOScale(Vector3.one, _scaleInterval).SetAutoKill(true) // 0.5秒でスケール1にする
                    .OnComplete(() => StartBlinking(_contrllerUi));    // スケールアップ完了後に点滅開始
                break;
            case IndicationUiType.ContorllerMove:
                canvasGrop.alpha = 1;
                _leverUi.gameObject.SetActive(false);
                _contrllerUi.gameObject.SetActive(true);
                _throttleUi.gameObject.SetActive(false);
                _contrllerUi.sprite = _indicationUiScriptableObject.ContorllerMove;
                // スケールを0から1にアニメーションさせ、完了したら点滅を開始
                _indicationUiCanvas.transform.localScale = Vector3.zero; // 初期スケールを0に設定
                tweener = _indicationUiCanvas.transform.DOScale(Vector3.one, _scaleInterval).SetAutoKill(true) // 0.5秒でスケール1にする
                    .OnComplete(() => StartBlinking(_contrllerUi));    // スケールアップ完了後に点滅開始
                break;
            case IndicationUiType.PushThrottle:
                canvasGrop.alpha = 1;
                _leverUi.gameObject.SetActive(false);
                _contrllerUi.gameObject.SetActive(false);
                _throttleUi.gameObject.SetActive(true);
                _throttleUi.sprite = _indicationUiScriptableObject.PushThrottle;
                // スケールを0から1にアニメーションさせ、完了したら点滅を開始
                _indicationUiCanvas.transform.localScale = Vector3.zero; // 初期スケールを0に設定
                tweener = _indicationUiCanvas.transform.DOScale(Vector3.one, _scaleInterval).SetAutoKill(true) // 0.5秒でスケール1にする
                    .OnComplete(() => StartBlinking(_throttleUi));    // スケールアップ完了後に点滅開始
                break;
            case IndicationUiType.PullThrottle:
                canvasGrop.alpha = 1;
                _leverUi.gameObject.SetActive(false);
                _contrllerUi.gameObject.SetActive(false);
                _throttleUi.gameObject.SetActive(true);
                _throttleUi.sprite = _indicationUiScriptableObject.PullThrottle;
                // スケールを0から1にアニメーションさせ、完了したら点滅を開始
                _indicationUiCanvas.transform.localScale = Vector3.zero; // 初期スケールを0に設定
                tweener = _indicationUiCanvas.transform.DOScale(Vector3.one, _scaleInterval).SetAutoKill(true) // 0.5秒でスケール1にする
                    .OnComplete(() => StartBlinking(_throttleUi));    // スケールアップ完了後に点滅開始
                break;
            case IndicationUiType.ThrottleTrigger:
                canvasGrop.alpha = 1;
                _leverUi.gameObject.SetActive(false);
                _contrllerUi.gameObject.SetActive(false);
                _throttleUi.gameObject.SetActive(true);
                _throttleUi.sprite = _indicationUiScriptableObject.ThrottleTrigger;
                // スケールを0から1にアニメーションさせ、完了したら点滅を開始
                _indicationUiCanvas.transform.localScale = Vector3.zero; // 初期スケールを0に設定
                tweener = _indicationUiCanvas.transform.DOScale(Vector3.one, _scaleInterval).SetAutoKill(true) // 0.5秒でスケール1にする
                    .OnComplete(() => StartBlinking(_throttleUi));    // スケールアップ完了後に点滅開始
                break;
            case IndicationUiType.Toggle:
                canvasGrop.alpha = 1;
                _leverUi.gameObject.SetActive(true);
                _contrllerUi.gameObject.SetActive(false);
                _throttleUi.gameObject.SetActive(false);
                _leverUi.sprite = _indicationUiScriptableObject.Toggle;
                // スケールを0から1にアニメーションさせ、完了したら点滅を開始
                _indicationUiCanvas.transform.localScale = Vector3.zero; // 初期スケールを0に設定
                tweener = _indicationUiCanvas.transform.DOScale(Vector3.one, _scaleInterval).SetAutoKill(true) // 0.5秒でスケール1にする
                    .OnComplete(() => StartBlinking(_leverUi));    // スケールアップ完了後に点滅開始
                break;
        }

    }

    private void StartBlinking(Image image)
    {
        if (_blinkCoroutine == null)
        {
            _blinkCoroutine = StartCoroutine(BlinkImage(image, _indicationUiDuration));
        }
    }

    private void StopBlinking()
    {
        if(_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
    }

    private IEnumerator BlinkImage(Image image, float duration)
    {
        while (true) // 無限ループで点滅させる
        {
            // フェードアウト
            yield return FadeImage(image, 1f, _indicationUiMinAlpha, duration);
            // フェードイン
            yield return FadeImage(image, _indicationUiMinAlpha, 1f, duration);
        }
    }

    private IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration)
    {
        Color color = image.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            image.color = color;
            yield return null;
        }

        color.a = endAlpha;
        image.color = color;
    }


    int _iconTest = 0;
    public void IconTest()
    {
        int i = _iconTest % 10;
        ChangeIndicationUi((IndicationUiType)Enum.ToObject(typeof(IndicationUiType), i));
        _iconTest++;
    }

    public void IconDeleteTest()
    {
        ChangeIndicationUi((IndicationUiType)Enum.ToObject(typeof(IndicationUiType), 0));
    }
}


/////////////////////////ここから下は関係ない///////////////////

// else
// {
//     //ファンネル
//     List<GameObject> fannels = new List<GameObject>();
//     //ボスのx軸
//     float bossXPosition = 0f;
//     float bossYPosition = 0f;
//     
//     //ここから
//     //軸の判定
//     string bossDirection = null;
//     
//     //ボス戦用のUi処理
//     for (int i = 0; i < _enemies.Count; i++)
//     {
//         AgentScript agent = _enemies[i].GetComponent<AgentScript>();
//
//         //ボスオブジェクトを設定する
//         if (_bossGameObject == null && agent.IsBoss)
//         {
//             _bossGameObject = agent.gameObject;
//         }
//
//         if(_bossGameObject != null)
//         {
//             if(agent.IsBoss)
//             {
//                 Vector3 enemyDir = _enemies[i].transform.position;
//                 //敵の高さとプレイヤーの高さを合わせる
//                 enemyDir.y = _player.position.y;
//                 enemyDir = _enemies[i].transform.position - _player.position;
//                 Vector3 enemy = _enemies[i].transform.position;
//                 enemy.y = _player.position.y;
//                 enemy = _player.position - _enemies[i].transform.position;
//                 //プレイヤーに対するボスの位置を取得
//                 //各軸の絶対値を取得
//                 float absX = Mathf.Abs(enemy.x);
//                 float absz = Mathf.Abs(enemy.z);
//                 
//                 Debug.Log(bossDirection);
//                 enemyDir = Quaternion.Inverse(_player.rotation) * enemyDir; // ベクトルをプレイヤーに合わせて回転
//                 enemyDir = Vector3.ClampMagnitude(enemyDir, _raderLength); // ベクトルの長さを制限
//                 //赤点の位置を決める
//                 agent.RectTransform.anchoredPosition3D = new Vector3(enemyDir.x * _radius + _offset.x, enemyDir.z * _radius + _offset.y, _offset.z);
//                 bossXPosition = enemyDir.x * _radius + _offset.x;
//                 bossYPosition = enemyDir.z * _radius + _offset.y;
//                 
//             } //ボスの位置を決める
//             else
//             {
//                 fannels.Add(agent.gameObject);
//                 //Vector3 enemyDir = _enemies[i].transform.position;
//                 //ボスの位置を決める
//                 //enemyDir.z = _bossGameObject.transform.position.z;
//                 //enemyDir = _enemies[i].transform.position - _bossGameObject.transform.position;
//
//                 ////y軸を固定する
//                 //enemyDir.y = _hight;
//
//                 //enemyDir = Quaternion.Inverse(_player.rotation) * enemyDir; // ベクトルをプレイヤーに合わせる
//                 ////赤点の位置を決める
//                 //agent.RectTransform.anchoredPosition3D = new Vector3(enemyDir.x * _radius * _widthLeverage + _offset.x, enemyDir.y * _radius + _offset.y, _offset.z);
//             }//ファンネルの位置を決める
//         }
//     }
//
//     //ファンネルの位置を決める
//     //ファンネルをボスのx軸から近い順に並べ替える
//     var sortedFunnels = fannels.OrderBy(enemy => Mathf.Abs(enemy.transform.position.x - _bossGameObject.transform.position.x)).ToArray();
//     //敵の総数を計算
//     int totalFunnels = sortedFunnels.Length;
//
//     float rightLastXPosition = float.MinValue; //右側に最後に入れた物
//     float leftLastXPosition = float.MinValue; //左側で最後に入れた物
//     float centerXPosition = float.MinValue;
//     int leftCount = 1;
//     int rightCount = 1;
//
//
//     if (bossDirection == "Forward")
//     {
//         //真ん中を決める
//         for(int i = 0; i < totalFunnels; i++)
//         {
//             AgentScript agent = sortedFunnels[i].GetComponent<AgentScript>();
//             Vector3 enemyDir = sortedFunnels[i].transform.position;
//             // ボスから敵への方向を計算
//             enemyDir = sortedFunnels[i].transform.position - _bossGameObject.transform.position;
//
//             //y軸を決める
//             enemyDir.y = _hight + enemyDir.y * _hightLeverage;
//             float adjustment = 0f;
//             //x軸を決める
//             if (i == 0)
//             {
//                 //最後に配置した位置を設定
//                 rightLastXPosition = enemyDir.x;
//                 leftLastXPosition = enemyDir.x;
//
//                 //赤点の位置を決める
//                 agent.RectTransform.anchoredPosition3D = new Vector3(bossXPosition, bossYPosition + _hight, _offset.z);
//             }
//             else
//             {
//                 if (enemyDir.x >= 0)
//                 {
//                     //enemyDir.x = rightLastXPosition + _widthInterval;
//                     //rightLastXPosition += _widthInterval;// 最後の位置を更新
//                     //赤点の位置を決める
//                     agent.RectTransform.anchoredPosition3D = new Vector3(
//                         bossXPosition - _widthInterval * leftCount, bossYPosition + _hight,
//                         _offset.z);
//                     leftCount++;
//                 }
//                 else
//                 {
//                     //enemyDir.x = leftLastXPosition - _widthInterval; // 最後の位置を更新
//                     //leftLastXPosition -= _widthInterval;
//                     agent.RectTransform.anchoredPosition3D = new Vector3(
//                         bossXPosition + _widthInterval * rightCount, bossYPosition + _hight,
//                         _offset.z);
//                     rightCount++;
//                 }
//             }
//         }
//     }

//     //enemyDir.x += adjustment;
//         //enemyDir = Vector3.ClampMagnitude(enemyDir, _raderLength); // ベクトルの長さを制限
//         //// プレイヤーの向きに合わせてベクトルを回転
//         //enemyDir = Quaternion.Inverse(_player.rotation) * enemyDir;
//
//         ////赤点の位置を決める
//         //agent.RectTransform.anchoredPosition3D = new Vector3(enemyDir.x * _radius * _widthLeverage + _offset.x, enemyDir.y * _radius + _offset.y, _offset.z);
// }