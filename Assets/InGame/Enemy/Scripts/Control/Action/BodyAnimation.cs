using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy.Control
{
    // 再生するアニメーションのキー
    public enum AnimationKey 
    { 
        Idle, 
        Left, 
        Right, 
        Attack, 
        Broken 
    };

    /// <summary>
    /// アニメーションさせる。
    /// </summary>
    public class BodyAnimation
    {
        private class Param
        {
            // 再生終了時のコールバック
            public UnityAction Callback;

            private float _playTime;

            public Param(string s)
            {
                _playTime = 0;
                Hash = Animator.StringToHash(s);
            }

            // 再生時間
            public float PlayTime
            {
                get => _playTime;
                set => _playTime = Mathf.Max(0, value);
            }

            // ハッシュ値
            public int Hash { get; }
        }

        private Animator _animator;

        // 再生中の時間とハッシュ値を管理
        private Dictionary<AnimationKey, Param> _params;

        public BodyAnimation(Animator animator)
        {
            _animator = animator;

            Setup();
        }

        // 辞書で管理する
        private void Setup()
        {
            _params = new Dictionary<AnimationKey, Param>
            {
                { AnimationKey.Idle, new Param(Const.IdleAnimationName) },
                { AnimationKey.Left, new Param(Const.LeftAnimationName) },
                { AnimationKey.Right, new Param(Const.RightAnimationName) },
                { AnimationKey.Attack, new Param(Const.AttackAnimationName) },
                { AnimationKey.Broken, new Param(Const.BrokenAnimationName) }
            };
        }   

        /// <summary>
        /// 行動に沿ったアニメーションを再生
        /// </summary>
        public void Play(AnimationKey key, UnityAction callback = null)
        {
            if (_params.TryGetValue(key, out Param param))
            {
                _animator.Play(param.Hash);

                // アニメーションの再生終了、再生時間が0になった際のコールバックを登録
                param.Callback = null;
                param.Callback = callback;

                // 現状再生時間が必要なのは攻撃と破壊のみ。
                if (key == AnimationKey.Attack) param.PlayTime = EnemyParams.Debug.AttackAnimationPlayTime;
                else if (key == AnimationKey.Broken) param.PlayTime = EnemyParams.Debug.BrokenAnimationPlayTime;
                else param.PlayTime = 0;

                // 現状アニメーションはどれか1つしか再生しないので
                // 他のアニメーションの再生時間カウントを0にする。
                foreach (KeyValuePair<AnimationKey, Param> p in _params)
                {
                    if (p.Key == key) continue;

                    // 他のアニメーションをキャンセルするため、再生時間を0にしている。
                    // 次の再生時間の更新のタイミングでコールバックが呼ばれるので注意。
                    p.Value.PlayTime = 0;
                }
            }
        }

        /// <summary>
        /// アニメーションが再生中かチェック
        /// </summary>
        public bool IsPlaying(AnimationKey key)
        {
            if (_params.TryGetValue(key, out Param param))
            {
                return param.PlayTime > 0;
            }
            else return false;
        }

        /// <summary>
        /// 他のアニメーションを再生中かチェック
        /// </summary>
        public bool IsOtherPlaying(AnimationKey key)
        {
            foreach(KeyValuePair<AnimationKey, Param> p in _params)
            {
                if (p.Key == key) continue;

                if (p.Value.PlayTime > 0) return true;
            }

            return false;
        }

        /// <summary>
        /// アニメーションの再生時間を更新。
        /// 再生時間が0になった場合はコールバック。
        /// </summary>
        public void PlayTime()
        {
            foreach(KeyValuePair<AnimationKey, Param> p in _params)
            {
                p.Value.PlayTime -= Time.deltaTime;

                // アニメーションが再生されている場合は、再生終了時にこの条件を満たす。
                // コールバックが登録されている場合は再生。
                if (p.Value.PlayTime <= 0)
                {
                    p.Value.Callback?.Invoke();
                    p.Value.Callback = null;
                }
            }
        }

        /// <summary>
        /// 非表示になる際の後始末。
        /// </summary>
        public void Cleaningup()
        {
            // コールバックを登録解除
            foreach (KeyValuePair<AnimationKey, Param> p in _params)
            {
                p.Value.Callback = null;
            }

            // 警告対策のためAnimatorを無効化
            _animator.enabled = false;
        }
    }
}
