using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// 撃破されたステート
    /// </summary>
    public class BrokenState : State
    {
        private BodyAnimation _bodyAnimation;

        // 一度だけアニメーションを再生するためのフラグ
        private bool _isAnimationPlaying;

        public BrokenState(BodyAnimation bodyAnimation)
        {
            _bodyAnimation = bodyAnimation;
        }

        public override StateKey Key => StateKey.Broken;

        protected override void Enter()
        {
            _isAnimationPlaying = false;
        }

        protected override void Exit()
        {
            _isAnimationPlaying = false;
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            if (!_isAnimationPlaying)
            {
                _isAnimationPlaying = true;
                _bodyAnimation.Play(Const.AnimationName.Broken);

                // 現状、アバターマスクが機能していないので、BaseLayerのアニメーションが再生されない。
            }
        }
    }
}
