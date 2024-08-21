using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel
{
    public class Perception
    {
        public Perception(RequiredRef requiredRef)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        /// <summary>
        /// 各値を更新。
        /// </summary>
        public void Update()
        {
            Vector3 bd = Ref.Boss.transform.position - Ref.Transform.position;
            Ref.BlackBoard.BossDirection = bd.normalized;
            Ref.BlackBoard.BossSqrDistance = bd.sqrMagnitude;

            Vector3 pd = Ref.Player.position - Ref.Transform.position;
            Ref.BlackBoard.PlayerDirection = pd.normalized;
            Ref.BlackBoard.PlayerSqrDistance = pd.sqrMagnitude;
        }

        /// <summary>
        /// ボス本体の左右もしくは周囲に展開させるための命令を黒板に書き込む。
        /// </summary>
        public void ExpandOrder()
        {
            ExpandMode mode = Ref.FunnelParams.ExpandMode;
            Ref.BlackBoard.ExpandOffset = ExpandOffset(mode);

            Ref.BlackBoard.Expand.Order();
        }

        // オフセットの位置を計算して返す。
        private static Vector3 ExpandOffset(ExpandMode mode)
        {
            if (mode == ExpandMode.Trace)
            {
                const float MaxHeight = 8.0f;
                const float MinHeight = 6.0f;
                const float MaxSide = 6.0f;
                const float MinSide = 4.0f;

                float sin = Mathf.Sin(2 * Mathf.PI * Random.value);
                float cos = Mathf.Cos(2 * Mathf.PI * Random.value);
                float dist = Random.Range(MinSide, MaxSide);
                float h = Random.Range(MinHeight, MaxHeight);
                int lr = Random.value <= 0.5f ? 1 : -1;
                
                return new Vector3(cos * dist * lr, h, sin * dist * lr);
            }
            else
            {
                const float Height = 10.0f;
                const float Side = 5.0f;

                int lr = default;
                if (mode == ExpandMode.Right) lr = 1;
                else if (mode == ExpandMode.Left) lr = -1;

                return new Vector3(lr * Side, Height, 0);
            }
        }

        /// <summary>
        /// 攻撃命令を黒板に書き込む。
        /// </summary>
        public void FireOrder()
        {

        }
    }
}
