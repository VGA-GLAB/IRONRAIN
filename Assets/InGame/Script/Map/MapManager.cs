using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("プレイヤー"), SerializeField, Tooltip("プレイヤー")]
    private Transform _playerTransform;
    [Header("マップオブジェクトリスト"), SerializeField, Tooltip("マップオブジェクトリスト")]
    private List<GameObject> _mapObjectsList;

    [Header("チェックポイントリスト"), SerializeField, Tooltip("チェックポイントリスト")]
    private List<Transform> _checkPointList;
    
   // [Header("")]
    
    // Start is called before the first frame update
    void Start()
    {
        //初期表示のマップをアクティブにする
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}