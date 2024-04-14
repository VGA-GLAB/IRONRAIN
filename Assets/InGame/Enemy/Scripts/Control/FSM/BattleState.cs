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

        // 移動と攻撃の両方がアニメーションを再生し、アバターマスクで組み合わさる。
        private MoveApply _moveApply;
        private AttackStream _attackStream;

        public BattleState(BlackBoard blackBoard, BodyMove move, BodyRotate rotate, BodyAnimation animation, 
            IEquipment weapon)
        {
            _blackBoard = blackBoard;
            _moveApply = new MoveApply(Choice.Chase, blackBoard, move, rotate, animation);
            _attackStream = new AttackStream(blackBoard, animation, weapon);
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
            if (StateExtensions.IsExit(_blackBoard))
            {
                TryChangeState(stateTable[StateKey.Idle]);
            }
            else
            {
                _moveApply.Run();
                _attackStream.Update();
            }
        }

        public override void Destroy()
        {
            _attackStream.ReleaseCallback();
        }
    }
}