using Enemy.DebugUse;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemy.Control.Boss
{
    [DefaultExecutionOrder(-1)]
    public class BossStage : MonoBehaviour
    {
        [Header("レーンの数")]
        [SerializeField] private int _laneQuantity = 36;
        [Header("ステージの半径")]
        [SerializeField] private float _radius = 30;

        private Lane[] _lanes;
        // 動くこと前提で、この点を中心とした円形のレーンを作る。
        private Transform _p;

        /// <summary>
        /// 点Pの位置。
        /// </summary>
        public Vector3 PointP => _p != null ? _p.position : Vector3.zero;
        /// <summary>
        /// 円状に配置されたレーン。
        /// 各レーンはインデックスを指定することで取得できる。
        /// </summary>
        public IReadonlyLane[] Lanes => _lanes;

        private void Start()
        {
            CreateTempPivot();
            CreateLanes();
        }

        private void Update()
        {
            foreach (Lane l in _lanes) l.Update();
        }

        // 仮の点Pを生成し、スクリプトで移動させる。
        // 本来はインスペクタから割り当てて、手づけのアニメーションで移動させる。
        private void CreateTempPivot()
        {
            _p = new GameObject("BossStage_TempPivot").transform;
            _p.AddComponent<BossStagePivotTempMove>();
        }

        // 原点を基準に円状にレーンを作成する。
        private void CreateLanes()
        {
            _lanes = new Lane[_laneQuantity];
            float rad = 2 * Mathf.PI / _laneQuantity;
            for (int i = 0; i < _lanes.Length; i++)
            {
                float sin = Mathf.Sin(rad * i);
                float cos = Mathf.Cos(rad * i);
                // 計算量を削減するため原点を中心とした位置を計算しておき
                // 毎フレーム点Pの位置をオフセットとして足す。
                Vector3 point = new Vector3(cos, 0, sin) * _radius;
                _lanes[i] = new Lane(_p, point);
            }
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
                GizmosUtils.WireCircle(l.LanePoint, 0.1f, Color.magenta);
            }
        }
    }
}
