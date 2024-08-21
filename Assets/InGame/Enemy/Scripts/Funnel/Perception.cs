using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel
{
    public class Perception
    {
        private Vector3? _buffer;

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
            // 計算したオフセットを黒板に1度だけ書き込む。
            if (_buffer != null)
            {
                Ref.BlackBoard.ExpandOffset = _buffer;
                _buffer = null;
            }

            Vector3 bd = Ref.Boss.transform.position - Ref.Transform.position;
            Ref.BlackBoard.BossDirection = bd.normalized;

            Vector3 pd = Ref.Player.position - Ref.Transform.position;
            Ref.BlackBoard.PlayerDirection = pd.normalized;
        }

        /// <summary>
        /// ボス本体の左右もしくは周囲に展開するオフセットを計算。
        /// 黒板に書き込まれるのは次のUpdateのタイミング。
        /// </summary>
        public void ExpandOffset()
        {
            ExpandMode mode = Ref.FunnelParams.ExpandMode;
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
                _buffer = new Vector3(cos * dist * lr, h, sin * dist * lr);
            }
            else
            {
                const float Height = 10.0f;
                const float Side = 5.0f;

                if (mode == ExpandMode.Right) _buffer = new Vector3(Side, Height, 0);
                else if (mode == ExpandMode.Left) _buffer = new Vector3(-Side, Height, 0);
            }
        }
    }
}
