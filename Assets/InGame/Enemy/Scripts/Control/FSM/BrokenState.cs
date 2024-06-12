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
        private Effector _effector;
        private BlackBoard _blackBoard;

        // 一度だけアニメーションを再生するためのフラグ
        private bool _isAnimationPlaying;

        public BrokenState(BodyAnimation bodyAnimation, Effector effector, BlackBoard blackBoard)
        {
            _bodyAnimation = bodyAnimation;
            _effector = effector;
            _blackBoard = blackBoard;
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

                _effector.Play(EffectKey.Destroyed, _blackBoard);

                // 現状、アバターマスクが機能していないので、BaseLayerのアニメーションが再生されない。
            }
        }
    }
}
