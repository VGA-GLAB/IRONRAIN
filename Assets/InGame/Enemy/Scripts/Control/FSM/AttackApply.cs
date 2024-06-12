using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// アニメーション含めた攻撃の流れを制御する。
    /// </summary>
    public class AttackApply
    {
        private BlackBoard _blackBoard;
        private BodyAnimation _animation;
        private IEquipment _equipment;

        public AttackApply(BlackBoard blackBoard, BodyAnimation animation, IEquipment equipment)
        {
            _blackBoard = blackBoard;
            _animation = animation;

            if (equipment == null) Debug.LogWarning($"装備無し: {blackBoard.Name}");
            else _equipment = equipment;

            FireAnimationEventCallback(BodyAnimation.CallBackControl.Add);
            FireAnimationEndCallback(BodyAnimation.CallBackControl.Add);
        }

        /// <summary>
        /// 黒板に書き込まれている行動から攻撃のみを実行
        /// </summary>
        public void Update()
        {
            // 攻撃可能なタイミングになった場合、攻撃するまで毎フレーム書き込まれる。
            // Brain側はアニメーションの状態を把握していないので、ここで調整する必要がある。
            while (_blackBoard.ActionOptions.TryDequeue(out ActionPlan plan))
            {
                // 攻撃以外の行動は弾く
                if (plan.Choice != Choice.Attack) continue;

                _equipment.PlayAttackAnimation(_animation);
            }
        }

        /// <summary>
        /// 登録したコールバックを解除
        /// </summary>
        public void ReleaseCallback()
        {
            FireAnimationEventCallback(BodyAnimation.CallBackControl.Remove);
            FireAnimationEndCallback(BodyAnimation.CallBackControl.Remove);
        }

        // アニメーションイベントに攻撃処理のコールバックを登録/解除
        private void FireAnimationEventCallback(BodyAnimation.CallBackControl control)
        {
            _animation.AnimationEventCallback(
                AnimationEvent.Key.Fire, 
                Attack, 
                control
                );

            // 攻撃処理
            void Attack()
            {
                if (_equipment == null) return;
                if (!_blackBoard.IsAlive) return;
                
                _equipment.Attack(_blackBoard);
                _blackBoard.LastAttackTime = Time.time;
            }
        }

        // 攻撃アニメーション終了時のコールバックに登録/解除
        private void FireAnimationEndCallback(BodyAnimation.CallBackControl control)
        {
            _animation.AnimationEventCallback(
                AnimationEvent.Key.FireAnimationEnd,
                () => _equipment.PlayAttackEndAnimation(_animation),
                control
                );
        }
    }
}