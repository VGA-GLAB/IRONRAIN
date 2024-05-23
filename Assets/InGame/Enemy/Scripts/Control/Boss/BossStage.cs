using Enemy.DebugUse;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
{
    public class Lane
    {
        public Lane(Vector3 point)
        {
            Point = point;
        }

        public Vector3 Point { get; set; }
    }

    [DefaultExecutionOrder(-1)]
    public class BossStage : MonoBehaviour
    {
        // 動くこと前提で、この点を中心とした円形のレーンを作る。
        [SerializeField] private Transform _p;
        [Header("レーンの数")]
        [SerializeField] private int _laneQuantity = 36;
        [Header("ステージの半径")]
        [SerializeField] private float _radius = 30;

        private Lane[] _lanes;

        private void Start()
        {
            _lanes = new Lane[_laneQuantity];
            float rad = 2 * Mathf.PI / _laneQuantity;
            for (int i = 0; i < _lanes.Length; i++)
            {
                float sin = Mathf.Sin(rad * i);
                float cos = Mathf.Cos(rad * i);
                Vector3 p = _p.position + new Vector3(cos, 0, sin) * _radius;
                _lanes[i] = new Lane(p);
            }
        }

        private void Update()
        {

        }

        private void OnDrawGizmos()
        {
            DrawLanes();
        }

        private void DrawLanes()
        {
            if (_lanes == null) return;

            foreach (Lane l in _lanes)
            {
                GizmosUtils.WireCircle(l.Point, 0.1f, Color.magenta);
            }
        }
    }
}
