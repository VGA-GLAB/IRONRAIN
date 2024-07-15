using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RaderMap : MonoBehaviour
{
    /// <summary>
    /// 敵のリスト
    /// </summary>
    private List<GameObject> _enemies = new List<GameObject>();
    /// <summary>
    /// 敵UIのリスト
    /// </summary>
    public Dictionary<GameObject, Image> EnemyMaps = new Dictionary<GameObject, Image>();
    [SerializeField, Tooltip("プレイヤーの位置")] private Transform _player;
    [SerializeField, Tooltip("UIの真ん中")] private Image _center;
    [SerializeField, Tooltip("レーダーの大きさ")] private float _raderLength = 30f;
    [SerializeField, Tooltip("半径")] private float _radius = 6f;
    [SerializeField, Tooltip("ロックオン可能距離")] private float _rockonDis = 100f;
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

    private MouseMultilockSystem _mouseMultilockSystem;

    private MultilockSystem _multilockSystem;

    [SerializeField] private bool _isMouse = true;

    private PokeInteractionBase _pokeInteractionBase;
    // Start is called before the first frame update
    void Start()
    {
        _offset = _center.GetComponent<RectTransform>().anchoredPosition3D;
        _mouseMultilockSystem = GameObject.FindObjectOfType<MouseMultilockSystem>();
        _pokeInteractionBase = FindObjectOfType<PokeInteractionBase>();
    }

    void Update()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            AgentScript _agent = _enemies[i].GetComponent<AgentScript>();

            Vector3 enemyDir = _enemies[i].transform.position;
            //敵の高さとプレイヤーの高さを合わせる
            enemyDir.y = _player.position.y;
            enemyDir = _enemies[i].transform.position - _player.position;

            enemyDir = Quaternion.Inverse(_player.rotation) * enemyDir; // ベクトルをプレイヤーに合わせて回転
            enemyDir = Vector3.ClampMagnitude(enemyDir, _raderLength); // ベクトルの長さを制限

            //赤点の位置を決める
            _agent.RectTransform.anchoredPosition3D = new Vector3(enemyDir.x * _radius + _offset.x, enemyDir.z * _radius + _offset.y, _offset.z);
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
            _enemies.Add(enemy);
            _pokeInteractionBase.AddIcon(enemyUi.gameObject);
        }
    }

    /// <summary>
    /// エネミーが倒された時に呼ばれるメソッド
    /// </summary>
    /// <param name="enemy"></param>
    public void DestroyEnemy(GameObject enemy)
    {
        if (_isMouse)
        {
            if(_mouseMultilockSystem.LockOnEnemy.Contains(enemy))
            {
                _mouseMultilockSystem.EnemyDestory(EnemyMaps[enemy].gameObject);
            }
        }
        else
        {
            if(_multilockSystem.LockOnEnemy.Contains(enemy))
            {
                _multilockSystem.EnemyDestory(EnemyMaps[enemy].gameObject);
            }
        }

        if (EnemyMaps.ContainsKey(enemy))
        {
            _pokeInteractionBase.RemoveIcon(EnemyMaps[enemy].gameObject);
            Destroy(EnemyMaps[enemy].gameObject);
            EnemyMaps.Remove(enemy);
            _enemies.Remove(enemy);
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
        for (int i = 0; i < _enemies.Count; i++)
        {
            //視野角内にいるのかを判定する
            if (!IsVisible(_enemies[i].gameObject))
                continue;

            float distance = Vector3.Distance(_enemies[i].transform.position, _player.transform.position);
            if (distance < nearDistance)
            {
                nearDistance = distance;
                _nearEnemy = _enemies[i].gameObject;
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
        EnemyMaps[agentScript.gameObject].color = agentScript._rockonColor;
        
        //前のターゲットと違うかを判定
        if (nearEnemy.obj != _nowRockEnemy)
        {
            _nowRockEnemy = nearEnemy.obj;
            //ターゲットが切り替わる音を出す
            CriAudioManager.Instance.SE.Play("SE", "SE_Targeting");
        }
        _enemyDistance = nearEnemyDis;
    }

    /// <summary>
    /// Panelを押したときのロックオン処理
    /// </summary>
    /// <param name="enemyUi"></param>
    public void PanelRock(GameObject enemyUi)
    {
        //var ui = enemyUi.GetComponent<EnemyUi>();
        var enemyObj = enemyUi.gameObject;
        var enemyAgent = enemyObj.GetComponent<AgentScript>();
        if(!enemyAgent.IsDefault)
        {
            //全てのエネミーのロックオンを外す
            ResetUi();
        }
        else
        {
            //全てのエネミーのロックオンを外す
            ResetUi();
            //パネルタッチでのロックオン状態にする
            if (!EnemyMaps.ContainsKey(enemyAgent.gameObject))
                return;
            enemyAgent.IsDefault = false;
            enemyAgent.IsRockon = true;
            EnemyMaps[enemyAgent.gameObject].color = enemyAgent._rockonColor;
            _nowRockEnemy = enemyAgent.gameObject;
            _enemyDistance = Vector3.Distance(enemyAgent.gameObject.transform.position, _player.transform.position);
            
            //ターゲットが切り替わる音を出す
            CriAudioManager.Instance.SE.Play("SE", "SE_Targeting");
        }  
    }

    /// <summary>
    /// すべてのUIをリセットする
    /// </summary>
    private void ResetUi()
    {
        //全てのエネミーのロックオンを外す
        foreach (var enemy in _enemies)
        {
            var agent = enemy.GetComponent<AgentScript>();
            agent.IsRockon = false;
            agent.IsDefault = true;
            EnemyMaps[enemy].color = agent._defultColor;
        }
    }
    //視野角をギズモ化
    private void OnDrawGizmos()
    {
        // 視界の範囲（正面及び左右の端）をギズモとして描く
        Vector3 selfDir = _origin.forward;
        Vector3 rightBorder = Quaternion.Euler(0, _sightAngle / 2, 0) * selfDir; //右端
        Vector3 leftBorder = Quaternion.Euler(0, -1 * _sightAngle / 2, 0) * selfDir; //左端
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(_origin.transform.position, selfDir * _rockonDis);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(_origin.transform.position, rightBorder * _rockonDis);
        Gizmos.DrawRay(_origin.transform.position, leftBorder * _rockonDis);
    }


    // <summary>
    // マルチロックオン処理
    // </summary>
    public void MultiLockon(HashSet<GameObject> enemys)
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
            EnemyMaps[agentScript.gameObject].color = agentScript._rockonColor;
        }
    }
}