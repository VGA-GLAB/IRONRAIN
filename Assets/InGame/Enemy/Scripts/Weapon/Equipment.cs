using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 敵の武器の基底クラス
    /// </summary>
    public class Equipment : Character
    {
        /// <summary>
        /// 最後に攻撃の当たり判定を出したタイミング。
        /// アニメーションイベントの呼び出しを装備者側が知ることが出来る。
        /// </summary>
        public float LastAttackTiming { get; protected set; }

        /// <summary>
        /// 描画のみを切る。
        /// </summary>
        public void RendererDisable()
        {
            foreach (Renderer r in _renderers)
            {
                r.enabled = false;
            }
        }
    }
}
