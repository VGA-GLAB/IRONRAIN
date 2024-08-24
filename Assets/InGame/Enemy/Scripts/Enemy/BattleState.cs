using Enemy.Extensions;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 雑魚3種それぞれのBattleStateの基底クラス。
    /// </summary>
    public abstract class BattleState : PlayableState
    {
        // レーン移動するための値。
        private float _delay;
        private float _save;

        public BattleState(RequiredRef requiredRef) : base(requiredRef)
        {
        }

        protected sealed override void Enter() 
        {
            ResetMoveParams();
            OnEnter();
        }
        protected sealed override void Exit()
        {
            OnExit();
        }      
        protected sealed override void Stay() 
        {
            PlayDamageSE();
            if (ExitIfDeadOrTimeOver()) return;
            MoveToSlot();

            StayIfBattle();
        }

        protected abstract void OnEnter();
        protected abstract void OnExit();

        /// <summary>
        /// ダメージ音、死亡または時間切れで遷移、アニメーション付きの移動。
        /// これらが処理された後、装備している武器毎に動きが違う。
        /// </summary>
        protected abstract void StayIfBattle();

        /// <summary>
        /// スロットの位置に座標を変更。
        /// </summary>
        protected void MoveToSlot()
        {
            Vector3 before = Ref.Body.Position;

            LookAtPlayer();
            Move();
            
            Vector3 after = Ref.Body.Position;

            // 移動前後の位置を比較して左右どちらに移動したかを判定する。
            if (IsMoving()) MoveAnimation(after - before);
        }

        // プレイヤーがレーンを移動したら多少遅れて敵も同じように移動する。
        private void Move()
        {
            // ディレイの値は -n ~ 0 の範囲をとる。
            // Lerpの t の値が0以下の場合は、返る値が a と等しいので、ディレイが0以下の間は移動を行わない。
            float bodyX = Ref.Body.Position.x;
            float slotX = Ref.BlackBoard.Slot.Point.x;
            float lerped = Mathf.Lerp(bodyX, slotX, Time.deltaTime + _delay);

            // レーンの幅以上の横移動をしたかつ、ディレイが0になり、敵もレーン移動し始めた場合。
            const float LaneWidth = 1.0f; // 手動。
            if (Mathf.Abs(slotX - _save) > LaneWidth && _delay >= 0)
            {
                ResetMoveParams();
            }

            _delay += Time.deltaTime;
            _delay = Mathf.Min(0, _delay);

            Vector3 sp = Ref.BlackBoard.Slot.Point;
            sp.x = lerped;
            Ref.Body.Warp(sp);
        }

        // レーン移動までのディレイの値で移動しているかを判定する。
        private bool IsMoving()
        {
            return _delay >= 0;
        }

        // プレイヤーがレーン移動を開始する度にリセットする。
        private void ResetMoveParams()
        {
            _delay = -Ref.EnemyParams.LaneChangeDelay;
            _save = Ref.BlackBoard.Slot.Point.x;
        }

        // プレイヤー方向に向く。
        private void LookAtPlayer()
        {
            Vector3 dir = Ref.BlackBoard.PlayerDirection;
            dir.y = 0;
            Ref.Body.LookForward(dir);
        }

        // スロットの位置へ向かうベクトルを返す。
        private Vector3 MovementPerFrame()
        {
            Vector3 v = VectorExtensions.Homing(
                Ref.BlackBoard.Area.Point,
                Ref.BlackBoard.Slot.Point,
                Ref.BlackBoard.SlotDirection,
                0.5f // 適当。
                );
            float dt = Ref.BlackBoard.PausableDeltaTime;
            return v * dt;
        }

        /// <summary>
        /// 通常は攻撃範囲にプレイヤーがいるかつ、次の攻撃タイミングが来ていた場合は攻撃。
        /// 手動攻撃の場合は攻撃命令を受けていれば攻撃。
        /// </summary>
        protected bool IsAttack()
        {
            SpecialCondition condition = Ref.EnemyParams.SpecialCondition;
            if (condition == SpecialCondition.ManualAttack)
            {
                Trigger attack = Ref.BlackBoard.OrderedAttack;
                return attack.IsWaitingExecute();
            }
            else
            {
                Trigger attack = Ref.BlackBoard.Attack;
                return attack.IsWaitingExecute() && CheckAttackRange();
            }
        }

        // プレイヤーが攻撃範囲内にいるかチェック。
        private bool CheckAttackRange()
        {
            foreach (Collider c in Ref.BlackBoard.FovStay)
            {
                if (c.CompareTag(Const.PlayerTag)) return true;
            }

            return false;
        }

        /// <summary>
        /// 攻撃のアニメーション再生処理を呼んだタイミングで同時に呼ぶ。
        /// 黒板に攻撃したことを書き込む。
        /// </summary>
        protected void AttackTrigger()
        {
            Ref.BlackBoard.OrderedAttack.Execute();
            Ref.BlackBoard.Attack.Execute();
        }
    }
}
