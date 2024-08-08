using Enemy.Extensions;
using UnityEngine;

namespace Enemy.DebugUse
{
    /// <summary>
    /// デバッグ用のステータスを表示するUI
    /// </summary>
    public class DebugStatusUI
    {
        private Transform _transform;
        private EnemyParams _params;
        private BlackBoard _blackBoard;

        public DebugStatusUI(RequiredRef requiredRef)
        {
            _transform = requiredRef.Transform;
            _params = requiredRef.EnemyParams;
            _blackBoard = requiredRef.BlackBoard;
        }

        /// <summary>
        /// ギズモにステータスを描画
        /// </summary>
        public void Draw()
        {
            HpBar();
            LifeTime();
        }

        // HPバー
        private void HpBar()
        {
            Vector3 p = _transform.position + Vector3.up * 5.0f;
            float f = 1.0f * _blackBoard.Hp / _params.MaxHp;
            f = Mathf.Clamp01(f);

            GizmosUtils.Plane(p, 2.0f, 0.3f, ColorExtensions.DarkGray);
            GizmosUtils.Plane(p, 2.0f * f, 0.3f, Color.white);
        }

        // 生存時間
        private void LifeTime()
        {
            Vector3 p = _transform.position + Vector3.up * 4.8f;
            float f = 1.0f * _blackBoard.LifeTime / _params.LifeTime;
            f = Mathf.Clamp01(f);

            GizmosUtils.Plane(p, 2.0f, 0.1f, ColorExtensions.DarkGray);
            GizmosUtils.Plane(p, 2.0f * f, 0.1f, Color.red);
        }
    }
}
