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
    [Tooltip("‰½”Ô–Ú‚Ìƒ|ƒCƒ“ƒg‚©")]
    public int PointNum;
}
