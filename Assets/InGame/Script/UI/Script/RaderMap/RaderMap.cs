﻿using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RaderMap : MonoBehaviour
{
    /// <summary>
    /// 敵のリスト
    /// </summary>
    //private List<AgentScript> _enemys = new List<AgentScript> ();
    private List<GameObject> _enemys = new List<GameObject>();
    /// <summary>
    /// 敵UIのリスト
    /// </summary>
    private Dictionary<GameObject, Image> _enemyMaps = new Dictionary<GameObject, Image>();
    [SerializeField, Tooltip("プレイヤーの位置")] private Transform _player;
    [SerializeField, Tooltip("UIの真ん中")] private Image _center;
    [SerializeField, Tooltip("レーダーの大きさ")] private float _raderLength = 30f;
    [SerializeField, Tooltip("半径")] private float _radius = 6f;
    [SerializeField, Tooltip("マルチロック距離")] private float _multilockDis = 10f;
    /// <summary>
    /// Centerからのオフセット
    /// </summary>
    private Vector3 _offset;
    /// <summary>
    /// 一番近い敵のゲームオブジェクト
    /// </summary>
    private GameObject _nearEnemy;
    // Start is called before the first frame update
    void Start()
    {
        ////敵をすべて取得する
        //GameObject[] objects = GameObject.FindGameObjectsWithTag("Enemy");
        //foreach(GameObject obj in objects)
        //{
        //    AgentScript _agent = obj.GetComponent<AgentScript>();
        //    _agent.RaderMap = this;
        //    _agent.RectTransform = Instantiate(_agent.Image, _center.transform.parent).GetComponent<RectTransform>();
        //    _enemys.Add(obj.GetComponent<AgentScript>());
        //}//エネミーを取得する

        _offset = _center.GetComponent<RectTransform>().anchoredPosition3D;
    }

    void Update()
    {
        for (int i = 0; i < _enemys.Count; i++)
        {
            AgentScript _agent = _enemys[i].GetComponent<AgentScript>();

            Vector3 enemyDir = _enemys[i].transform.position;
            //敵の高さとプレイヤーの高さを合わせる
            enemyDir.y = _player.position.y;
            enemyDir = _enemys[i].transform.position - _player.position;

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
        _enemyMaps.Add(enemy, enemyUi);
        agent.RectTransform = enemyUi.GetComponent<RectTransform>();
        //_enemys.Add(enemy.GetComponent<AgentScript>());
        _enemys.Add(enemy);
    }

    /// <summary>
    /// エネミーが倒された時に呼ばれるメソッド
    /// </summary>
    /// <param name="enemy"></param>
    public void DestroyEnemy(GameObject enemy)
    {
        Destroy(_enemyMaps[enemy].gameObject);
        _enemyMaps.Remove(enemy);
        _enemys.Remove(enemy);
    }

    /// <summary>
    /// プレイヤーから１番近い敵のゲームオブジェクトを返すメソッド
    /// </summary>
    /// <returns>最も近い敵を返す</returns>
    public GameObject NearEnemy()
    {
        float nearDistance = float.MaxValue;
        
        //エネミーとの距離を判定する
        for (int i = 0; i < _enemys.Count; i++)
        {
            float distance = Vector3.Distance(_enemys[i].transform.position, _player.transform.position);
            if (distance < nearDistance)
            {
                nearDistance = distance;
                _nearEnemy = _enemys[i].gameObject;
            }
        }
        return _nearEnemy;
    }

    /// <summary>
    /// 一番近い敵をロックオンする
    /// </summary>
    public void NearEnemyLockon()
    {
        //全てのエネミーのロックオンを外す
        foreach(var enemy in _enemys)
        {
            var agent = enemy.GetComponent<AgentScript>();
            agent.IsRockon = false;
            _enemyMaps[enemy].color = agent._defultColor;
        }
        
        var nearEnemy = NearEnemy();
        AgentScript agentScript = nearEnemy.GetComponent<AgentScript>();
        agentScript.IsRockon = true;
        _enemyMaps[agentScript.gameObject].color = agentScript._rockonColor;
    }

    /// <summary>
    /// マルチロックオン処理
    /// </summary>
    public void MaltiLockon()
    {
        //全てのエネミーのロックオンを外す
        foreach (var enemy in _enemys)
        {
            var agent = enemy.GetComponent<AgentScript>();
            agent.IsRockon = false;
        }

        //エネミーとの距離を判定する
        for (int i = 0; i < _enemys.Count; i++)
        {
            float distance = Vector3.Distance(_enemys[i].transform.position, _player.transform.position);
            if (distance < _multilockDis)
            {
                AgentScript agentScript = _enemys[i].GetComponent<AgentScript>();
                agentScript.IsRockon = true;
            }
        }
    }
}