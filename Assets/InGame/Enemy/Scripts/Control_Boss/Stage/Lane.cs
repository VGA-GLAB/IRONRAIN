using UnityEngine;

namespace Enemy.Control.Boss
{
    /// <summary>
    /// 読み取り専用。
    /// ボスステージの円状のレーン。
    /// </summary>
    public interface IReadonlyLane
    {
        /// <summary>
        /// 中心点Pの位置から半径の長さぶん離れた、レーン上の点の位置。
        /// </summary>
        public Vector3 LanePoint { get; }
    }

    public class Lane : IReadonlyLane
    {
        private Transform _p;
        private Vector3 _point;
        private Vector3 _lanePoint;

        public Lane(Transform p, Vector3 point)
        {
            _p = p;
            _point = point;
            Update();
        }

        public Vector3 LanePoint => _lanePoint;

        // Updateとギズモへの描画で毎フレーム2回計算しないようにする。
        public void Update() => _lanePoint = _point + _p.position;
    }
}
