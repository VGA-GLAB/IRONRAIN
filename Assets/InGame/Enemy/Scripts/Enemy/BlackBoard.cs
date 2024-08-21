using Enemy.FSM;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Perception層とAction層が互いに書き込む。
    /// 命令されたら1度だけ実行し、実行したことを命令する側で検知する。
    /// </summary>
    public enum Trigger
    {
        None,     // 命令されておらず、実行していない。
        Ordered,  // 命令された。
        Executed, // 実行した。
    }

    /// <summary>
    /// コメントに特に記述が無い場合は、Perception層が書き込む。
    /// </summary>
    public class BlackBoard : IReadonlyBlackBoard
    {
        public BlackBoard(string name = "")
        {
            ID = System.Guid.NewGuid();
            Name = name;
            FovStay = new HashSet<Collider>();
        }

        // 外部から個体毎の判定をするための値。
        public System.Guid ID { get; private set; }
        public string Name { get; private set; }

        // このフレームで視界に捉えている対象。
        public HashSet<Collider> FovStay { get; private set; }

        // 位置関係を計算する用途の円状のエリア。Startでインスタンスが確保される。
        public Area Area { get; set; }
        public Area PlayerArea { get; set; }
        public Area Slot { get; set; }

        // 生成後、画面に表示させる際の位置。
        public Vector3? SpawnPoint { get; set; }

        // 現在実行中のステート。Action層が書き込む。
        public StateKey CurrentState { get; set; }

        // スロットの方向
        public Vector3 SlotDirection { get; set; }
        // スロットとの距離の二乗
        public float SlotSqrDistance { get; set; }
        // プレイヤーの方向
        public Vector3 PlayerDirection { get; set; }
        // プレイヤーとの距離
        public float PlayerDistance { get; set; }

        // 残り戦闘時間
        public float LifeTime { get; set; }
        // 現在の体力
        public int Hp { get; set; }

        // 命令やメソッド呼び出しで攻撃させるトリガー。
        public Trigger OrderedAttack { get; set; }
        // 時間経過で攻撃させるトリガー。
        public Trigger Attack { get; set; }

        // このフレームに受けたダメージ量。
        public int Damage { get; set; }
        // このフレームで自身にダメージを与えた武器。
        public string DamageSource { get; set; }

        // プレイヤーを検知しているかのフラグ。
        public bool IsPlayerDetect { get; set; }
        // プレイヤーを検知後、戦闘開始位置まで接近したかのフラグ。
        public bool IsApproachCompleted { get; set; }
        // ポーズ中かのフラグ。
        public bool IsPause { get; set; }
        // ゲームがQTE実行中かのフラグ。
        public bool IsQteRunning { get; set; }
        // 自身がQTEの対象かのフラグ。
        public bool IsQteTargeted { get; set; }
        // 生存中かのフラグ。
        public bool IsAlive => Hp > 0;
        // 退場が完了し、後処理を呼んで消しても良い状態のフラグ。Action層が書き込む。
        public bool IsCleanupReady { get; set; }

        // スローやポーズ処理を反映したDeltaTime。
        public float PausableDeltaTime => Time.deltaTime * PausableTimeScale;
        // スローやポーズ処理を行う用途のTimeScale。
        public float PausableTimeScale => ProvidePlayerInformation.TimeScale * (IsPause ? 0 : 1);
    }
}
