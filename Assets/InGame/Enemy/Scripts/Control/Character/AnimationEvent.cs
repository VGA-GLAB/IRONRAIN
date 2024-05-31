using UnityEngine;
using UnityEngine.Events;

namespace Enemy.Control
{
    /// <summary>
    /// このスクリプトはAnimatorと同じオブジェクトにアタッチすること。
    /// </summary>
    public class AnimationEvent : MonoBehaviour
    {
        public enum Key { Fire, FireAnimationEnd, DamageAnimationEnd }

        // 攻撃アニメーション中、弾や判定を出すタイミングで呼ばれる。
        UnityAction OnFire;
        // 攻撃アニメーションが終了したタイミングで呼ばれる。
        UnityAction OnFireAnimationEnd;
        // ダメージアニメーションが終了したタイミングで呼ばれる。
        UnityAction OnDamageAnimationEnd;

        private void OnDestroy()
        {
            // 登録されているコールバックを全て解除
            OnFire = null;
            OnFireAnimationEnd = null;
        }

        /// <summary>
        /// コールバックを登録
        /// </summary>
        public void Register(Key key, UnityAction action)
        {
            if (key == Key.Fire) OnFire += action;
            if (key == Key.FireAnimationEnd) OnFireAnimationEnd += action;
            if (key == Key.DamageAnimationEnd) OnDamageAnimationEnd += action;
        }

        /// <summary>
        /// コールバックを解除
        /// </summary>
        public void Release(Key key, UnityAction action)
        {
            if (key == Key.Fire) OnFire -= action;
            if (key == Key.FireAnimationEnd) OnFireAnimationEnd -= action;
            if (key == Key.DamageAnimationEnd) OnDamageAnimationEnd -= action;
        }

        /// <summary>
        /// 攻撃アニメーション中の弾や判定を出すタイミング。
        /// アニメーションイベントとして割り当てる。
        /// </summary>
        public void Fire()
        {
            OnFire?.Invoke();
        }

        /// <summary>
        /// 攻撃アニメーションの再生終了タイミング。
        /// アニメーションイベントとして割り当てる。
        /// </summary>
        public void FireAnimationEnd()
        {
            OnFireAnimationEnd?.Invoke();
        }

        /// <summary>
        /// ダメージアニメーションの再生終了タイミング。
        /// アニメーションイベントとして割り当てる。
        /// </summary>
        public void DamageAnimationEnd()
        {
            OnDamageAnimationEnd?.Invoke();
        }
    }
}
