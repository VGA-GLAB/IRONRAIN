using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 接近範囲内にプレイヤーが入っているかを調べる。
    /// </summary>
    public class ApproachSensor
    {
        private Transform _transform;
        private EnemyParams _params;

        public ApproachSensor(Transform transform, EnemyParams enemyParams)
        {
            _transform = transform;
            _params = enemyParams;
        }

        /// <summary>
        /// 値の更新
        /// </summary>
        public void Update(BlackBoard blackBoard)
        {
            // 自身とプレイヤーの距離が接近開始距離か
            if (blackBoard.TransformToPlayerDistance < _params.Advance.Distance)
            {
                blackBoard.IsPlayerDetected = true;
            }
        }

        /// <summary>
        /// 範囲を描画
        /// </summary>
        public void DrawRange()
        {
            GizmosUtils.WireCircle(_transform.position, _params.Advance.Distance, ColorExtensions.ThinGreen);
        }
    }
}