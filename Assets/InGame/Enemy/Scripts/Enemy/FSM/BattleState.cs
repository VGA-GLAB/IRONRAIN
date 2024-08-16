using Enemy.Extensions;
using UnityEngine;

namespace Enemy.FSM
{
    /// <summary>
    /// 接近状態と雑魚3種類それぞれの戦闘状態の基底クラス。
    /// Stayで呼ぶ前提のメソッドのみを持ち、呼び出し自体は行わない。
    /// </summary>
    public class BattleState : State
    {
        protected EnemyParams _params;
        protected BlackBoard _blackBoard;
        protected Body _body;
        protected BodyAnimation _animation;

        public BattleState(StateRequiredRef requiredRef) : base(requiredRef.States)
        {
            _params = requiredRef.EnemyParams;
            _blackBoard = requiredRef.BlackBoard;
            _body = requiredRef.Body;
            _animation = requiredRef.BodyAnimation;
        }

        protected override void Enter() { }
        protected override void Exit() { }
        protected override void Stay() { }

        /// <summary>
        /// ダメージを受けた場合に音を再生。
        /// </summary>
        protected void PlayDamageSE()
        {
            string seName = "";
            if (_blackBoard.DamageSource == Const.PlayerRifleWeaponName) seName = "SE_Damage_02";
            else if (_blackBoard.DamageSource == Const.PlayerMeleeWeaponName) seName = "SE_PileBunker_Hit";
            
            if (seName != "") AudioWrapper.PlaySE(seName);
        }

        /// <summary>
        /// 死亡もしくは撤退の場合は、アイドル状態を経由して退場するステートに遷移。
        /// </summary>
        protected bool BattleExit()
        {
            if (_blackBoard.Hp <= 0) { TryChangeState(StateKey.Broken); return true; }
            else if (_blackBoard.LifeTime <= 0) { TryChangeState(StateKey.Escape); return true; }

            return false;
        }

        /// <summary>
        /// スロットの位置へアニメーションしつつ移動。
        /// </summary>
        protected void MoveToSlot(float speed)
        {
            // 移動前後の位置を比較して左右どちらに移動したかを判定する。
            Vector3 before = _body.Position;
            MoveToSlot(MovementPerFrame(speed));
            LookAtPlayer();
            Vector3 after = _body.Position;

            MoveAnimation(after - before);
        }

        // スロットの位置へ向かうベクトルを返す。
        private Vector3 MovementPerFrame(float speed)
        {
            Vector3 v = VectorExtensions.Homing(
                _blackBoard.Area.Point,
                _blackBoard.Slot.Point,
                _blackBoard.SlotDirection,
                _params.Other.ApproachHomingPower
                );

            return v * speed * _blackBoard.PausableDeltaTime;
        }

        // エリアの中心位置からスロット方向へ1フレームぶん移動した位置へワープさせる。
        // エリアの半径が小さすぎない限り、移動させても飛び出すことは無い。
        private void MoveToSlot(in Vector3 mpf)
        {
            if (mpf.sqrMagnitude >= _blackBoard.SlotSqrDistance)
            {
                _body.Warp(_blackBoard.Slot.Point);
            }
            else
            {
                _body.Warp(_blackBoard.Area.Point + mpf);
            }
        }

        // プレイヤーを向かせる。
        private void LookAtPlayer()
        {
            Vector3 dir = _blackBoard.PlayerDirection;
            dir.y = 0;

            _body.Forward(dir);
        }

        // 移動した方向ベクトルでアニメーションを制御。
        // z軸を前方向として、ベクトルのx成分の正負で左右どちらに移動したかを判定する。
        private void MoveAnimation(in Vector3 moveDir)
        {
            if (Mathf.Abs(moveDir.x) < Mathf.Epsilon)
            {
                _animation.SetBool(BodyAnimationConst.Param.IsLeftMove, false);
                _animation.SetBool(BodyAnimationConst.Param.IsRightMove, false);
            }
            else if (moveDir.x < 0)
            {
                _animation.SetBool(BodyAnimationConst.Param.IsLeftMove, false);
                _animation.SetBool(BodyAnimationConst.Param.IsRightMove, true);
            }
            else if (moveDir.x > 0)
            {
                _animation.SetBool(BodyAnimationConst.Param.IsLeftMove, true);
                _animation.SetBool(BodyAnimationConst.Param.IsRightMove, false);
            }
        }

        /// <summary>
        /// 通常は攻撃範囲にプレイヤーがいるかつ、次の攻撃タイミングが来ていた場合は攻撃。
        /// 手動攻撃の場合は攻撃命令を受けていれば攻撃。
        /// </summary>
        public bool IsAttack()
        {
            if (_params.SpecialCondition == SpecialCondition.ManualAttack)
            {
                return _blackBoard.OrderedAttack == Trigger.Ordered;
            }
            else
            {
                return _blackBoard.Attack ==Trigger.Ordered && CheckAttackRange();
            }
        }

        /// <summary>
        /// 攻撃のアニメーション再生処理を呼んだタイミングで同時に呼ぶ。
        /// 黒板に攻撃したことを書き込む。
        /// </summary>
        public void AttackTrigger()
        {
            _blackBoard.OrderedAttack = Trigger.Executed;
            _blackBoard.Attack = Trigger.Executed;
        }

        // プレイヤーが攻撃範囲内にいるかチェック。
        private bool CheckAttackRange()
        {
            foreach (Collider c in _blackBoard.FovStay)
            {
                if (c.CompareTag(Const.PlayerTag)) return true;
            }

            return false;
        }
    }
}
