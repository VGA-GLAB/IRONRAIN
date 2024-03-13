using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// アニメーションさせる。
    /// </summary>
    public class BodyAnimation
    {
        private Animator _animator;

        public BodyAnimation(Animator animator)
        {
            _animator = animator;
        }
    }
}
