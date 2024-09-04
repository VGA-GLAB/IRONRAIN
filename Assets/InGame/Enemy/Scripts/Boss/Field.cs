using IronRain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss
{
    public class Field
    {
        private Transform _pointP;
        private PlayerBossMoveModel _playerBossMoveModel;

        public Field(RequiredRef requiredRef, Transform pointP)
        {
            Ref = requiredRef;
            _pointP = pointP;
        }

        private RequiredRef Ref { get; }
        private int CurrentLaneIndex => Ref.BlackBoard.CurrentLaneIndex;
        private int PlayerLaneIndex => _playerBossMoveModel.CurrentRane.Value;
        public int Length => _playerBossMoveModel.LaneList.Count;

        /// <summary>
        /// ボス戦開始後、プレイヤー側でフィールドの構築が完了した場合は有効になる。
        /// </summary>
        public bool IsEnabled()
        {
            // ボス戦開始までnullになっているため、コンストラクタ時点では参照が確保できない。
            if (_playerBossMoveModel == null)
            {
                PlayerBossMove pbm = Ref.Player.GetComponentInChildren<PlayerBossMove>();
                if (pbm != null) _playerBossMoveModel = pbm.MoveModel;
            }

            return _playerBossMoveModel != null;
        }

        /// <summary>
        /// 現在の自身のレーンのワールド座標にオフセットを足したものを返す。
        /// </summary>
        public Vector3 GetCurrentLanePointWithOffset()
        {
            return GetLanePointWithOffset(CurrentLaneIndex);
        }

        /// <summary>
        /// 指定したレーンのワールド座標にオフセットを足したものを返す。
        /// </summary>
        public Vector3 GetLanePointWithOffset(int index)
        {
            Vector3 offset = Ref.BossParams.Position.HeightOffset;
            return PointP() + GetLane(index) + offset;
        }

        /// <summary>
        /// 点Pの位置を返す。
        /// </summary>
        public Vector3 PointP()
        {
            return _pointP.position;
        }

        /// <summary>
        /// 原点を中心とした現在のレーンを返す。
        /// </summary>
        public Vector3 GetCurrentLane()
        {
            return GetLane(CurrentLaneIndex);
        }

        /// <summary>
        /// プレイヤーのいるレーンを返す。
        /// </summary>
        public Vector3 GetPlayerLane()
        {
            return GetLane(PlayerLaneIndex);
        }

        /// <summary>
        /// 指定したレーンを返す。
        /// </summary>
        public Vector3 GetLane(int index)
        {
            return _playerBossMoveModel.LaneList[index];
        }

        /// <summary>
        /// プレイヤーと反対側のレーンの番号を返す。
        /// </summary>
        public int GetOtherSideLaneIndex()
        {
            return (PlayerLaneIndex + (Length / 2)) % Length;
        }

        /// <summary>
        /// 時計回りでの移動回数を返す。
        /// </summary>
        public int GetClockwiseMoveCount()
        {
            int t = GetOtherSideLaneIndex();
            return GetMoveCount(t, CurrentLaneIndex);
        }

        /// <summary>
        /// 反時計回りでの移動回数を返す。
        /// </summary>
        public int GetCounterClockwiseMoveCount()
        {
            int t = GetOtherSideLaneIndex();
            return GetMoveCount(CurrentLaneIndex, t);
        }

        /// <summary>
        /// 時計回りと反時計回り、少ない方の移動回数を返す。
        /// </summary>
        public int GetMinMoveCount()
        {
            int c = GetClockwiseMoveCount();
            int cc = GetCounterClockwiseMoveCount();
            return Mathf.Min(c, cc);
        }

        /// <summary>
        /// 時計回り、1つ左のレーンの番号を返す。
        /// </summary>
        public int GetLeftLaneIndex()
        {
            // 2点を指定して移動回数を求める計算と同じ。
            // 時計回りも反時計回りも、なぜか現在の地点から移動方向を指定しても隣のレーンが求まる。
            return GetMoveCount(CurrentLaneIndex, -1);
        }

        /// <summary>
        /// 反時計回り、1つ右のレーンの番号を返す。
        /// </summary>
        public int GetRightLaneIndex()
        {
            return GetMoveCount(CurrentLaneIndex, 1);
        }

        /// <summary>
        /// 円状に敷き詰められたレーンの2点間を移動する場合の移動回数を求める。
        /// </summary>
        public int GetMoveCount(int a, int b)
        {
            // 1,2,3...17,0のように、末尾から先頭に移動する際に、変化量が変わるのを考慮して計算する。
            // aとbを入れ替えると時計回り/反時計回りの移動回数が求まる。
            return (a - b + Length) % Length;
        }
    }
}
