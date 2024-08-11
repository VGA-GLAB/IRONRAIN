using Enemy.Boss.FSM;
using UnityEngine;

namespace Enemy.Boss
{
    /// <summary>
    /// キャラクターの情報を各層で共有する。
    /// </summary>
    public class BlackBoard : IReadonlyBlackBoard
    {
        public BlackBoard(string name = "")
        {
            ID = System.Guid.NewGuid();
            Name = name;
        }

        // 外部から個体毎の判定をするための値。
        public System.Guid ID { get; private set; }
        public string Name { get; private set; }

        // 位置関係を計算する用途の円状のエリア。Startでインスタンスが確保される。
        public Area Area { get; set; }
        public Area PlayerArea { get; set; }

        // 現在実行中のステート。Action層が書き込む。
        public StateKey CurrentState { get; set; } // まだ

        // 前方向
        public Vector3 Forward { get; set; }
        // 点Pの方向
        public Vector3 PointPDirection { get; set; }
        // 点Pとの距離
        public float PointPDistance { get; set; }
        // プレイヤーの方向
        public Vector3 PlayerDirection { get; set; }
        // プレイヤーの方向の二乗
        public float PlayerSqrDistance { get; set; }

        // 戦闘開始からの経過時間
        public float ElapsedTime { get; set; }
        // 次に遠距離攻撃が可能になる時間
        public float NextRangeAttackTime { get; set; }
        // 次に近距離攻撃が可能になる時間
        public float NextMeleeAttackTime { get; set; }

        // このフレームに受けたダメージ量。
        public int Damage { get; set; }
        // このフレームで自身にダメージを与えた武器。
        public string DamageSource { get; set; }

        // ファンネルを展開するタイミングのトリガー
        public bool FunnelExpandTrigger { get; set; }

        // ボス戦開始フラグ。
        public bool IsBossStarted { get; set; }
        // 登場演出が完了したフラグ。
        public bool IsAppearCompleted { get; set; }
        // プレイヤーが近接攻撃の範囲内にいるフラグ。
        public bool IsWithinMeleeRange { get; set; }
        // ファンネルのレーザーサイト表示されるフラグ。
        public bool IsFunnelLaserSight { get; set; }
        // QTEイベントが開始されたフラグ。
        public bool IsQteEventStarted { get; set; }
        // QTEイベントの進捗。
        public QteEventState.Step OrderdQteEventStep { get; set; }
        // QTEを経てボスが破壊されたフラグ。
        public bool IsBroken { get; set; }

        // 現状ボス戦ではポーズ処理が無いが一応。
        public float PausableDeltaTime => Time.deltaTime;
    }
}
