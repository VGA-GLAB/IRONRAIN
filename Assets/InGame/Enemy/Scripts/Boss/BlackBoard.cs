using Enemy.Boss.FSM;
using UnityEngine;

namespace Enemy.Boss
{
    /// <summary>
    /// コメントに特に記述が無い場合は、Perception層が書き込む。
    /// </summary>
    public class BlackBoard : IReadonlyBlackBoard
    {
        public BlackBoard(string name)
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
        public StateKey CurrentState { get; set; }

        // 点Pの方向
        public Vector3 PointPDirection { get; set; }
        // 点Pとの距離
        public float PointPDistance { get; set; }
        // プレイヤーの方向
        public Vector3 PlayerDirection { get; set; }
        // プレイヤーの距離の二乗
        public float PlayerSqrDistance { get; set; }

        // 戦闘開始からの経過時間
        public float ElapsedTime { get; set; }

        // このフレームに受けたダメージ量。
        public int Damage { get; set; }
        // このフレームで自身にダメージを与えた武器。
        public string DamageSource { get; set; }

        // ファンネルを展開。
        public Trigger FunnelExpand { get; set; }
        // 遠距離攻撃。
        public Trigger RangeAttack { get; set; }
        // 近接攻撃。
        public Trigger MeleeAttack { get; set; }

        // ボス戦開始フラグ。
        public bool IsBossStarted { get; set; }
        // 登場演出が完了したフラグ。
        public bool IsAppearCompleted { get; set; }
        // プレイヤーが近接攻撃の範囲内にいるフラグ。
        public bool IsWithinMeleeRange { get; set; }
        // ファンネルのレーザーサイト表示されるフラグ。
        public bool IsFunnelLaserSight { get; set; }
        // QTEイベントが開始されたフラグ。
        public bool IsQteStarted { get; set; }
        // QTEを行う位置に立っているかのフラグ。Action層が書き込む。
        public bool IsStandingOnQtePosition { get; set; }
        // QTEイベント、左腕を破壊する演出開始フラグ。
        public bool IsBreakLeftArm { get; set; }
        // QTEイベント、左腕破壊->鍔迫り合い1回目。
        public bool IsQteCombatReady { get; set; }
        // QTEイベント、鍔迫り合い1回目の入力がされたフラグ。
        public bool IsFirstCombatInputed { get; set; }
        // QTEイベント、鍔迫り合い2回目の入力がされたフラグ。
        public bool IsSecondCombatInputed { get; set; }
        // QTEイベント、パイルバンカーで貫く入力がされたフラグ。
        public bool IsPenetrateInputed { get; set; }
        // QTEを経てボスが破壊されたフラグ。
        public bool IsBroken { get; set; } // まだ

        // 現状ボス戦ではポーズ処理が無いが一応。
        public float PausableDeltaTime => Time.deltaTime;
    }
}
