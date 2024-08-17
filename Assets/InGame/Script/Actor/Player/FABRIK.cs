using System.Collections.Generic;
using UnityEngine;

public class FABRIK : MonoBehaviour
{
    [Header("骨")]
    [SerializeField] private List<Transform> _bones = new List<Transform>();
    [Header("目的地")]
    [SerializeField] private Transform _target;
    [Header("試行回数")]
    [SerializeField] private int _maxIteration = 5;

    [Header("===各ボーンの角度制限を保持するリスト (Vector3で各軸の制限)===")]
    [SerializeField] private List<Vector3> _minAngleLimits = new List<Vector3>();
    [SerializeField] private List<Vector3> _maxAngleLimits = new List<Vector3>();

    List<float> lengths = new List<float>();
    List<Vector3> positions = new List<Vector3>();

    void Awake()
    {
        // ボーンの長さ
        lengths.Clear();
        for (int i = 0; i < _bones.Count - 1; i++)
        {
            lengths.Add(Vector3.Distance(_bones[i].position, _bones[i + 1].position));
        }

        // ボーンの位置
        positions.Clear();
        foreach (var b in _bones)
        {
            positions.Add(b.position);
        }
    }

    void Update()
    {
        // 現在のボーン位置をコピーしてくる
        for (int i = 0; i < _bones.Count; i++)
        {
            positions[i] = _bones[i].position;
        }

        // FABRIKでボーン位置を推定
        var basePos = positions[0];
        var targetPos = _target.position;
        var prevDistance = 0.0f;
        for (int iter = 0; iter < _maxIteration; iter++)
        {
            // 収束チェック
            var distance = Vector3.Distance(positions[positions.Count - 1], targetPos);
            var change = Mathf.Abs(distance - prevDistance);
            prevDistance = distance;
            //ある程度
            if (distance < 1e-6 || change < 1e-8)
            {
                break;
            }

            // Backward
            positions[positions.Count - 1] = targetPos;
            for (int i = positions.Count - 1; i >= 1; i--)
            {
                var direction = (positions[i] - positions[i - 1]).normalized;
                positions[i - 1] = positions[i] - direction * lengths[i - 1];
            }

            // Forward
            positions[0] = basePos;
            for (int i = 0; i <= positions.Count - 2; i++)
            {
                var direction = (positions[i + 1] - positions[i]).normalized;
                positions[i + 1] = positions[i] + direction * lengths[i];
            }
        }

        // 推定したボーン位置から回転角を計算
        for (int i = 0; i < positions.Count - 1; i++)
        {
            var origin = _bones[i].position;
            var current = _bones[i + 1].position;
            var target = positions[i + 1];
            var delta = GetDeltaRotation(origin, current, target);
            //_bones[i].rotation = delta * _bones[i].rotation;

            _bones[i].rotation = delta * _bones[i].rotation;
        }
    }

    Quaternion GetDeltaRotation(Vector3 origin, Vector3 current, Vector3 target)
    {
        var beforeDirection = (current - origin).normalized;
        var afterDirection = (target - origin).normalized;
        return Quaternion.FromToRotation(beforeDirection, afterDirection);
    }
}