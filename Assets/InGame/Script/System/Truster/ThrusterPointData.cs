using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterPointData
{
    public ThrusterPointData(Vector3 position, int pointNum)
    {
        Position = position;
        PointNum = pointNum;
    }

    public Vector3 Position;
    [Tooltip("何番目のポイントか")]
    public int PointNum;
}
