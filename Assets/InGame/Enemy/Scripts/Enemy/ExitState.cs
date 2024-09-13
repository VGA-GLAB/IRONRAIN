using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 破壊もしくは撤退で画面から退場するステートの基底クラス。
    /// 共通して使うメソッドを持つだけ。
    /// </summary>
    public class ExitState : State<StateKey>
    {
        public ExitState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        protected RequiredRef Ref { get; }

        protected override void Enter() { }
        protected override void Exit() { }
        protected override void Stay() { }

        /// <summary>
        /// 退場の演出を行う基準となる位置を返す。
        /// 盾持ちとそれ以外で違う。
        /// </summary>
        protected Vector3 GetBasePosition()
        {
            if (Ref.EnemyParams.Type == EnemyType.Shield)
            {
                return Ref.BlackBoard.BrokenPosition;
            }
            else
            {
                return Ref.BlackBoard.Slot.Point;
            }
        }
    }
}
