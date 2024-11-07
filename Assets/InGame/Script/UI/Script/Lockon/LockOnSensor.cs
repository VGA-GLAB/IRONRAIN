using System.Collections.Generic;
using UnityEngine;

public class LockOnSensor : MonoBehaviour
{
    // 現在LockOnしているオブジェクト
    private GameObject _nowTarget;
    // 感知できる敵のリスト
    private List<GameObject> _enemyList;

    void Start()
    {
        _nowTarget = null;
        _enemyList = new List<GameObject>();
    }

    //colliderを使う場合の索敵処理
    private void OnTriggerStay(Collider col)
    {
        //敵が索敵範囲内に入ったら処理を行う
        if (col.CompareTag("Enemy") && !_enemyList.Contains(col.gameObject))
        {
            _enemyList.Add(col.gameObject);
        }

        if (_nowTarget == null)
        {
            _nowTarget = col.gameObject;
        }
    }

    //敵が索敵範囲を出たら行う処理
    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Enemy") && _enemyList.Contains(col.gameObject))
        {
            if (col.gameObject == _nowTarget)
            {
                _nowTarget = null;
            }
        }
        _enemyList.Remove(col.gameObject);
    }

    //colliderを使う場合の処理終了
    /// <summary>現在のロックオンオブジェクトを返すメソッド </summary>
    /// <returns>ロックオンされているオブジェクト</returns>
    public GameObject GetNowTarget()
    {
        return _nowTarget;
    }

    /// <summary>ロックオンオブジェクトがない場合にセットするメソッド</summary>
    public void SetNowTarget()
    {
        foreach (var enemy in _enemyList)
        {
            if (_nowTarget == null)
            {
                _nowTarget = enemy;
            }
        }
    }

    /// <summary>ロックオンオブジェクトを切り替える </summary>
    public void LockOnSwitch()
    {
        if (_enemyList.IndexOf(_nowTarget) != _enemyList.Count - 1)
        {
            _nowTarget = _enemyList[_enemyList.IndexOf(_nowTarget) + 1];
        }
        else
        {
            _nowTarget = _enemyList[0];
        }
    }

    /// <summary>マルチロック時に索敵範囲のエネミーを返す</summary>
    /// <returns>索敵範囲のエネミー</returns>
    public List<GameObject> GetEnemyList()
    {
        return _enemyList;
    }
}