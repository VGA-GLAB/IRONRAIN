using UnityEngine;

namespace Enemy
{
    public class BladeEquipment : MeleeEquipment
    {
        [SerializeField] private Animator _animator;

        protected override void OnOnEnable()
        {
            _animationEvent.OnBladeSwing += PlaySwingSE;
        }

        protected override void OnOnDisable()
        {
            _animationEvent.OnBladeSwing -= PlaySwingSE;
        }

        protected override void OnCollision()
        {
        }

        /// <summary>
        /// 刀を展開するアニメーション。
        /// </summary>
        public void Open()
        {
            _animator.SetTrigger(Const.Param.Open);
            _animator.ResetTrigger(Const.Param.Close);
        }

        /// <summary>
        /// 刀をしまうアニメーション。
        /// </summary>
        public void Close()
        {
            _animator.SetTrigger(Const.Param.Close);
            _animator.ResetTrigger(Const.Param.Open);
        }

        // 刀を振る際の音
        private void PlaySwingSE()
        {
            Vector3 p = transform.position;
            AudioWrapper.PlaySE(p, "SE_Sword");
        }
    }
}