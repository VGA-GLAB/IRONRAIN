using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("プレイヤー"), SerializeField, Tooltip("プレイヤー")]
    private Transform _playerTransform;
    [Header("マップオブジェクトリスト"), SerializeField, Tooltip("マップオブジェクトリスト")]
    private List<GameObject> _mapObjectsList;
    [Header("チェックポイントリスト"), SerializeField, Tooltip("チェックポイントリスト")]
    private List<Transform> _checkPointList;

    /// <summary>チェックポイントのカウンター</summary>
    private int _checkPointCount;
    
    // Start is called before the first frame update
    private void Start()
    {
        //初期表示のマップをアクティブにする
        for (int i = 0; i < _mapObjectsList.Count; i++)
        {
            if (i < 2)
                _mapObjectsList[i].SetActive(true);
            else
                _mapObjectsList[i].SetActive(false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_checkPointCount == _mapObjectsList.Count - 2)
            return;
        //マップを進める
        if (_checkPointList[_checkPointCount].position.z <= _playerTransform.position.z)
            ChangeMap();
    }

    /// <summary>
    /// マップの入れ替えをする処理
    /// </summary>
    private void ChangeMap()
    {
        _mapObjectsList[_checkPointCount].SetActive(false);
        _mapObjectsList[_checkPointCount + 2].SetActive(true);
        _checkPointCount++;
    }
}