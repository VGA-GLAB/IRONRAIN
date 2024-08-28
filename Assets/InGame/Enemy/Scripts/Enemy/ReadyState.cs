using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 画面に表示されたタイミング、接近開始までに必要な準備を行う。
    /// </summary>
    public class ReadyState : State<StateKey>
    {
        public ReadyState(RequiredRef requiredRef) : base(requiredRef.States) 
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Ready;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            // 画面に表示された次のフレームからPerceptionが動くので、このステートで合計3フレーム待つ。
            // このステートが無いと、ApproachのEnterのタイミングでスロットの計算が行われない。
            TryChangeState(StateKey.Approach);
        }
    }
}
