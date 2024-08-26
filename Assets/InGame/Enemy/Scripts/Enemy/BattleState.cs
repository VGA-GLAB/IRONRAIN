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
        // プレイヤーの移動量を測るため、スロットのx座標。
        private float _savedSlotX;
        // ホバリングさせるための値。
        private float _hovering;

        public BattleState(RequiredRef requiredRef) : base(requiredRef)
        {
        }

        protected sealed override void Enter() 
        {
            OnEnter();

            // プレイヤーの移動に遅れて左右移動させるため、初期値を設定。
            _delay = Ref.EnemyParams.LaneChangeDelay;
            _savedSlotX = Ref.BlackBoard.Slot.Point.x;

            Always();
        }
        protected sealed override void Exit()
        {
            Always();
            OnExit();
        }      
        protected sealed override void Stay() 
        {
            if (ExitIfDeadOrTimeOver()) return;

            Always();
            StayIfBattle();
        }

        protected abstract void OnEnter();
        protected abstract void OnExit();

        /// <summary>
        /// ダメージ音、死亡または時間切れで遷移、アニメーション付きの移動。
        /// これらが処理された後、装備している武器毎に動きが違う。
        /// </summary>
        protected abstract void StayIfBattle();

        private void Always()
        {
            PlayDamageSE();

            // ホバリングで上下に動かす。
            float h = Mathf.Sin(_hovering);
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _hovering += dt;
            Ref.Body.OffsetWarp(Vector3.up * h);

            // ディレイの値は -n ~ 0 の範囲をとる。
            // Lerpの t の値が0以下の場合は、返る値が a と等しいので、ディレイが0以下の間は移動を行わない。
            float bodyX = Ref.Body.Position.x;
            float slotX = Ref.BlackBoard.Slot.Point.x;
            float lerped = Mathf.Lerp(bodyX, slotX, Time.deltaTime - _delay);

            // レーンの幅以上の横移動をしたかつ、ディレイが0になり、敵もレーン移動し始めた場合。
            const float LaneWidth = 1.0f; // 手動。
            if (Mathf.Abs(slotX - _savedSlotX) > LaneWidth && _delay >= 0)
            {
                _delay = Ref.EnemyParams.LaneChangeDelay;
                _savedSlotX = Ref.BlackBoard.Slot.Point.x;
            }

            _delay += Time.deltaTime;
            _delay = Mathf.Min(0, _delay);

            Vector3 before = Ref.Body.Position;

            // 回転。
            Vector3 fwd = Ref.Body.Forward;
            Vector3 dir = Ref.BlackBoard.PlayerDirection;
            fwd.y = 0;
            dir.y = 0;
            Vector3 look = Vector3.Lerp(fwd, dir, Time.deltaTime);
            Ref.Body.LookForward(look);

            // 移動。
            Vector3 sp = Ref.BlackBoard.Slot.Point;
            sp.x = lerped;
            Ref.Body.Warp(sp);

            Vector3 after = Ref.Body.Position;

            // ディレイ後、移動前後の位置を比較して左右どちらに移動したかを判定する。
            bool isMoving = _delay >= 0;
            if (isMoving) MoveAnimation(after - before);
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
