using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// 移動しつつ攻撃するステート
    /// </summary>
    public class BattleState : State
    {
        private BlackBoard _blackBoard;

        private DamageApply _damageApply;
        // 移動と攻撃の両方がアニメーションを再生し、アバターマスクで組み合わさる。
        private MoveApply _moveApply;
        private AttackApply _attackApply;

        public BattleState(BlackBoard blackBoard, Body body, BodyAnimation animation, 
            IEquipment weapon)
        {
            _blackBoard = blackBoard;
            _moveApply = new MoveApply(Choice.Chase, blackBoard, body, animation);
            _attackApply = new AttackApply(blackBoard, animation, weapon);
        }

        public override StateKey Key => StateKey.Battle;

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {      
            // ダメージを受けてアニメーションを再生中は、死亡したり移動や攻撃をしない。
            // NOTE:ダメージ用のアニメーションが無いので、一通り動くまで作ったがテストできない。
            //if (_damageApply.IsPlaying()) return;

            if (IsExit())
            {
                TryChangeState(stateTable[StateKey.Idle]);
            }
            else
            {
                _moveApply.Run();
                _attackApply.Update();
            }
        }

        public override void Destroy()
        {
            _attackApply.ReleaseCallback();
        }

        // 死亡もしくは撤退をチェックする。
        private bool IsExit()
        {
            foreach (ActionPlan plan in _blackBoard.ActionOptions)
            {
                if (plan.Choice == Choice.Broken || plan.Choice == Choice.Escape)
                {
                    return true;
                }
            }

            return false;
        }
    }
}