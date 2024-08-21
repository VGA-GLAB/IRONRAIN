﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Funnel.FSM;

namespace Enemy.Funnel
{
    public class BlackBoard : IOwnerTime
    {
        public BlackBoard(string name = "")
        {
            ID = System.Guid.NewGuid();
            Name = name;
        }

        // 外部から個体毎の判定をするための値。
        public System.Guid ID { get; private set; }
        public string Name { get; private set; }

        // 現在実行中のステート。Action層が書き込む。
        public StateKey CurrentState { get; set; }

        // 展開する際のオフセット。
        public Vector3? ExpandOffset { get; set; }

        // ボスの方向
        public Vector3 BossDirection { get; set; }
        // プレイヤーの方向
        public Vector3 PlayerDirection { get; set; }

        // 現在の体力
        public int Hp { get; set; }
        // このフレームに受けたダメージ量。
        public int Damage { get; set; }
        // このフレームで自身にダメージを与えた武器。
        public string DamageSource { get; set; }

        // プレイヤーを検知しているかのフラグ。
        public bool IsPlayerDetect { get; set; }
        // 展開フラグ。
        public bool IsExpand { get; set; }
        // 生存中かのフラグ。
        public bool IsAlive => Hp > 0;
        // 退場が完了し、後処理を呼んで消しても良い状態のフラグ。Action層が書き込む。
        public bool IsCleanupReady { get; set; }

        // スローを反映したDeltaTime。
        public float PausableDeltaTime => Time.deltaTime * PausableTimeScale;
        public float PausableTimeScale => ProvidePlayerInformation.TimeScale;
    }
}
