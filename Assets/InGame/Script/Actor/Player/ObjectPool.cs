using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//ObjectPoolとはObjectを大量にInstantiateまたはDestroyするとパーフォーマンスが落ちることを防ぐためのデザインパターン
//弾幕ゲームやvampireSurvivorsのように大量のInstantiateまたはDestroyするゲームでは必須
//逆にあまり大量のInstantiateまたはDestroyしないゲームではObjectをあらかじめ生成して置いておくのにもメモリを使うためObjectPoolを使う必要はない

public class ObjectPool
{
    private Dictionary<PoolObjType, GameObject> _insPrefab = new();
    private List<Pool> _pool = new();

    /// <summary>
    /// 初回のObject生成
    /// </summary>
    /// <param name="poolObj"></param>
    /// <param name="insPos"></param>
    /// <param name="insNum"></param>
    public void CreateInitPool(GameObject poolObj, Transform insPos, int insNum, PoolObjType type) 
    {
        if (_insPrefab.ContainsKey(type))
        {

        }
        else 
        {
            _insPrefab.Add(type, poolObj);
        }

        for (int i = 0; i < insNum; i++) 
        {
            GameObject.Instantiate(poolObj, insPos);
            _pool.Add(new Pool(poolObj, type));
        }
    }

    /// <summary>
    /// オブジェクトを座標で使いたいときに呼び出す関数
    /// </summary>
    /// <param name="position">オブジェクトをスポーンさせる位置を指定する</param>
    /// <param name="objectType">オブジェクトの種類</param>
    /// <returns>生成したオブジェクト</returns>
    public GameObject UseObject(Vector2 position, PoolObjType objectType)
    {
        foreach (var pool in _pool)
        {
            if (pool.Object.activeSelf == false && pool.Type == objectType)
            {
                pool.Object.transform.position = position;
                pool.Object.SetActive(true);
                return pool.Object;
            }
        }

        //プールの中に該当するTypeのObjectがなかったら生成する
        var newObj = GameObject.Instantiate(_insPrefab[objectType], Vector3.zero, Quaternion.identity);
        newObj.transform.position = position;
        newObj.SetActive(true);
        _pool.Add(new Pool(newObj, objectType));

        Debug.LogWarning($"{objectType}のプールのオブジェクト数が足りなかったため新たにオブジェクトを生成します" +
       $"\nこのオブジェクトはプールの最大値が少ない可能性があります" +
       $"現在{objectType}の数は{_pool.FindAll(x => x.Type == objectType).Count}です");

        return newObj;
    }

    /// <summary> プールするObjを保存するための構造体 </summary>
    public struct Pool
    {
        public GameObject Object;
        public PoolObjType Type;

        public Pool(GameObject g, PoolObjType t)
        {
            Object = g;
            Type = t;
        }
    }
}

public enum PoolObjType 
{
    PlayerBullet,
}