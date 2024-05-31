using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// ダメージのアニメーションを再生する。
    /// </summary>
    public class DamageApply
    {
        private BlackBoard _blackBoard;
        private BodyAnimation _animation;

        bool _isPlaying;

        public DamageApply(BlackBoard blackBoard, BodyAnimation animation)
        {
            _blackBoard = blackBoard;
            _animation = animation;

            AnimationEventCallback(BodyAnimation.CallBackControl.Add);     
        }

        /// <summary>
        /// 毎フレーム呼び出す。
        /// ダメージを受けたタイミングでアニメーションを再生し、再生中かどうかを返す。
        /// </summary>
        public bool IsPlaying()
        {
            if (_blackBoard.CurrentFrameDamage > 0)
            {
                _animation.SetTrigger(Const.AnimationParam.DamagedTrigger);
                _isPlaying = true;
            }

            return _isPlaying;
        }

        // ダメージアニメーション終了のコールバックに登録/解除
        private void AnimationEventCallback(BodyAnimation.CallBackControl control)
        {
            _animation.AnimationEventCallback(
                AnimationEvent.Key.DamageAnimationEnd,
                () => _isPlaying = false,
                control
                );
        }
    }
}