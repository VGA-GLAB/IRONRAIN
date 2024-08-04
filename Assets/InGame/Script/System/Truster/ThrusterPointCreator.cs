using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 案１Pointを事前に生成
/// 案２左右のポイントを取得する度に生成
/// 案３事前に配置
/// </summary>
public class ThrusterPointContainer : MonoBehaviour
{
    private List<ThrusterPointData> _thrusterPointDataList = new();

    [SerializeField] private Transform _playerObj;
    [SerializeField] private Transform _insPos;
    [SerializeField] private Transform _centerPoint;
    [Header("生成するオブジェクト")]
    [SerializeField]
    private GameObject _createObject;
    [Header("生成するオブジェクトの数")]
    [SerializeField]
    private int _objCount = 40;

    [Tooltip("現在どこのポイントにいるか")]
    private int _nowPoint = 0;

    void Start()
    {
        InsThrusterPoint();
    }

    /// <summary>
    /// スラスターポイントを生成する
    /// </summary>
    private void InsThrusterPoint() 
    {
        // sin の周期は 2π
        var oneCycle = 2.0f * Mathf.PI;
        //中心点までの距離
        var distance = (_playerObj.transform.position - _centerPoint.position).sqrMagnitude;

        for (var i = 0; i < _objCount; ++i)
        {
            // 周期の位置 (1.0 = 100% の時 2π)
            var point = ((float)i / _objCount) * oneCycle;

            var x = Mathf.Cos(point) * distance;
            var z = Mathf.Sin(point) * distance;

            var position = new Vector3(x + _insPos.transform.position.x, _playerObj.transform.position.y, z + _insPos.transform.position.z);

            _thrusterPointDataList[i] = new ThrusterPointData(position, i);

            //Instantiate(
            //    _createObject,
            //    position,
            //    Quaternion.identity,
            //    transform
            //);
        }
    }

    /// <summary>
    /// 次の左側のポイントに移動する
    /// </summary>
    public ThrusterPointData NextLeftPoint(float distance) 
    {
        _nowPoint++;
        return PointPositionUpdate(distance);
        
    }

    /// <summary>
    /// 次の右側のポイントに移動する
    /// </summary>
    /// <returns></returns>
    public ThrusterPointData NextRightPoint(float distance) 
    {
        _nowPoint--;
        return PointPositionUpdate(distance);
    }

    /// <summary>
    /// ポイントのポジションを更新
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    private ThrusterPointData PointPositionUpdate(float distance) 
    {
        var pointData = _thrusterPointDataList[_nowPoint];
        var oneCycle = 2.0f * Mathf.PI;
        var point = (pointData.PointNum / _objCount) * oneCycle;
        var x = Mathf.Cos(point) * distance;
        var z = Mathf.Sin(point) * distance;
        var position = new Vector3(x + _insPos.transform.position.x, _playerObj.transform.position.y, z + _insPos.transform.position.z);
        pointData.Position = position;
        return pointData;
    }
}
