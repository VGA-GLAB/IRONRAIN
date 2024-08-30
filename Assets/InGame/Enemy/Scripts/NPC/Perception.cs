using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.NPC
{
    public class Perception
    {
        public Perception(RequiredRef requiredRef)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        /// <summary>
        /// 初期化。Startのタイミングで呼ぶ想定。
        /// </summary>
        public void InitializeOnStart()
        {
            // 生存時間
            Ref.BlackBoard.LifeTime = Ref.NpcParams.LifeTime;
        }

        /// <summary>
        /// 値を更新。
        /// </summary>
        public void Update()
        {
            Calculate();
            Overwrite();
        }

        // 必要な値を計算し、黒板に書き込む。
        private void Calculate()
        {
            BlackBoard bb = Ref.BlackBoard;

            if (bb.CurrentState == StateKey.Hide) return;

            // 目標へのベクトルを黒板に書き込む。
            Character target = Ref.NpcParams.Target;
            if (target != null)
            {
                Vector3 td = target.transform.position - Ref.Transform.position;
                bb.TargetDirection = td.normalized;
                bb.TargetSqrDistance = td.sqrMagnitude;
            }
            else
            {
                bb.TargetDirection = Vector3.forward;
                bb.TargetSqrDistance = 0;
            }

            // 生存時間を減らす。
            bb.LifeTime -= bb.PausableDeltaTime;
        }

        // 黒板の値を外部からの命令で上書きする。
        private void Overwrite()
        {
            //
        }

        /// <summary>
        /// 再生フラグを立て、黒板に書き込む。
        /// </summary>
        public void Play()
        {
            Ref.BlackBoard.IsPlay = true;
        }
    }
}
