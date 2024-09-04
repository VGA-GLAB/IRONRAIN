using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy.NPC
{
    public class BlackBoard : IOwnerTime
    {
        public BlackBoard(string name = "")
        {
            ID = System.Guid.NewGuid();
            Name = name;
        }

        // 現在実行中のステート。ステート側が書き込む。
        public StateKey CurrentState { get; set; }

        // 外部から個体毎の判定をするための値。
        public System.Guid ID { get; private set; }
        public string Name { get; private set; }

        // 残り生存時間
        public float LifeTime { get; set; } // まだ

        // 目標の方向
        public Vector3 TargetDirection { get; set; }
        // 目標の距離の二乗
        public float TargetSqrDistance { get; set; }

        // 再生フラグ。
        public bool IsPlay { get; set; }
        // 時間切れフラグ。
        public bool IsLifeTimeOver => LifeTime <= 0;

        // スローやポーズ処理を反映したFixedDeltaTime。
        public float PausableDeltaTime => Time.fixedDeltaTime * ProvidePlayerInformation.TimeScale;
    }
}
