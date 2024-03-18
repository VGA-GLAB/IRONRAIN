using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// デバッグ用のステータスを表示するUI
    /// </summary>
    public class DebugStatusUI
    {
        private Transform _transform;
        private EnemyParams _params;
        private BlackBoard _blackBoard;

        public DebugStatusUI(Transform transform, EnemyParams enemyParams, BlackBoard blackBoard)
        {
            _transform = transform;
            _params = enemyParams;
            _blackBoard = blackBoard;
        }

        /// <summary>
        /// ギズモにステータスを描画
        /// </summary>
        public void Draw()
        {
            HpBar();
        }

        // HPバー
        private void HpBar()
        {
            Vector3 p = _transform.position + Vector3.up * 5;
            float f = 1.0f * _blackBoard.Hp / _params.Tactical.MaxHp;
            f = Mathf.Clamp01(f);

            GizmosUtils.Plane(p, 2.0f, 0.3f, ColorExtensions.DarkGray);
            GizmosUtils.Plane(p, 2.0f * f, 0.3f, Color.white);
        }
    }
}
