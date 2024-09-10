using UnityEngine;

namespace Enemy
{
    public class BladeEquipment : MeleeEquipment
    {
        [SerializeField] private Animator _animator;

        protected override void OnCollision()
        {
            Vector3 p = transform.position;
            AudioWrapper.PlaySE(p, "SE_Sword");
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
    }
}