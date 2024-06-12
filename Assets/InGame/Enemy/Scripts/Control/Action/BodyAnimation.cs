using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy.Control
{
    /// <summary>
    /// アニメーションさせる。
    /// ObservableStateMachineTriggerコンポーネントがAnimatorにアタッチされている必要がある。
    /// 全てのステートはPlaySpeedというパラメータで速度を管理し、0で停止、1で再生する。
    /// 列挙型ではなく定数値の文字列で管理する。
    /// </summary>
    public class BodyAnimation
    {
        /// <summary>
        /// コールバックの登録/解除
        /// </summary>
        public enum CallBackControl { Add, Remove }

        // Animatorの各ステート
        private class State
        {
            // 再生開始時のコールバック
            public UnityAction OnPlayEnter;
            // 再生終了時のコールバック
            public UnityAction OnPlayExit;

            public State(string s)
            {
                Name = s;
                Hash = Animator.StringToHash(s);
                IsPlaying = false;
                OnPlayEnter = null;
                OnPlayExit = null;
            }

            public string Name { get; private set; }
            public int Hash { get; private set; }
            // 再生中かのフラグ
            public bool IsPlaying { get; set; }
        }

        private Animator _animator;
        private AnimationEvent _animationEvent;

        // 文字列の定数値で各ステートを管理する。
        private Dictionary<string, State> _states;

        public BodyAnimation(Animator animator, AnimationEvent animationEvent)
        {
            _animator = animator;
            _animationEvent = animationEvent;

            // 辞書で管理する
            _states = new Dictionary<string, State>
            {
                { Const.AnimationName.ApproachEnter, new State(Const.AnimationName.ApproachEnter) },
                { Const.AnimationName.ApproachStay, new State(Const.AnimationName.ApproachStay) },
                { Const.AnimationName.Idle, new State(Const.AnimationName.Idle) },
                { Const.AnimationName.Battle , new State(Const.AnimationName.Battle) },
                { Const.AnimationName.Aim, new State(Const.AnimationName.Aim) },
                { Const.AnimationName.Fire, new State(Const.AnimationName.Fire) },
                { Const.AnimationName.Reload, new State(Const.AnimationName.Reload) },
                { Const.AnimationName.Broken , new State(Const.AnimationName.Broken) },
            };

            foreach (ObservableStateMachineTrigger trigger in _animator.GetBehaviours<ObservableStateMachineTrigger>())
            {
                // ステート開始のタイミングをトリガー
                trigger.OnStateEnterAsObservable()
                    .Subscribe(a => OnStateEnter(a.StateInfo, a.LayerIndex))
                    .AddTo(animator);
                
                // ステート終了のタイミングをトリガー
                trigger.OnStateExitAsObservable()
                    .Subscribe(a => OnStateExit(a.StateInfo, a.LayerIndex))
                    .AddTo(animator);
            }
        }

        // ステート開始のコールバックを呼ぶ。
        private void OnStateEnter(AnimatorStateInfo info, int layerIndex)
        {
            foreach(KeyValuePair<string, State> p in _states)
            {
                // ハッシュ値でどのステートかを判定してコールバックを呼ぶ。
                if(p.Value.Hash == info.shortNameHash)
                {
                    p.Value.OnPlayEnter?.Invoke();
                    break;
                }
            }
        }

        // ステート終了のコールバックを呼ぶ。
        private void OnStateExit(AnimatorStateInfo info, int layerIndex)
        {
            foreach (KeyValuePair<string, State> p in _states)
            {
                // ハッシュ値でどのステートかを判定してコールバックを呼ぶ。
                if (p.Value.Hash == info.shortNameHash)
                {
                    p.Value.OnPlayExit?.Invoke();
                    break;
                }
            }
        }

        public bool IsStatePlaying(string stateName)
        {
            // Animatorの構造によって変えること。
            AnimatorStateInfo baseLayerState = _animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo upperLayerState = _animator.GetCurrentAnimatorStateInfo(1);

            // ハッシュ値で現在再生中のステートを特定し、そのステートが引数のステートと等しいかで判定。
            foreach (KeyValuePair<string, State> p in _states)
            {
                if (p.Value.Hash == baseLayerState.shortNameHash && p.Value.Name == stateName)
                {
                    return true;
                }

                if (p.Value.Hash == upperLayerState.shortNameHash && p.Value.Name == stateName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 行動に沿ったアニメーションを再生
        /// </summary>
        public void Play(string stateName)
        {
            if (_states.TryGetValue(stateName, out State state))
            {
                _animator.Play(state.Hash);
            }
            else Debug.LogWarning("アニメーションの再生に対応していない: " + stateName);
        }

        /// <summary>
        /// アニメーションの再生を一時停止。
        /// Updateの処理が一時停止したタイミングで呼ぶ。
        /// </summary>
        public void Pause()
        {
            _animator.SetFloat(Const.AnimationParam.PlaySpeed, 0);
        }

        /// <summary>
        /// アニメーションの再生を再開。
        /// Updateの処理が再開したタイミングで呼ぶ。
        /// </summary>
        public void Resume()
        {
            _animator.SetFloat(Const.AnimationParam.PlaySpeed, 1);
        }

        /// <summary>
        /// 再生速度を0~1の間で変更
        /// </summary>
        public void PlaySpeed(float value)
        {
            value = Mathf.Clamp01(value);
            _animator.SetFloat(Const.AnimationParam.PlaySpeed, value);
        }

        /// <summary>
        /// ステート開始時のコールバックを登録/解除
        /// </summary>
        public void EnterCallback(string stateName, UnityAction callback, CallBackControl control)
        {
            if (_states.TryGetValue(stateName, out State state))
            {
                if (control == CallBackControl.Add) state.OnPlayEnter += callback;
                else state.OnPlayEnter -= callback;
            }
            else Debug.LogWarning("ステート開始時のコールバックに対応していない: " + stateName);
        }

        /// <summary>
        /// ステート終了時のコールバックを登録/解除
        /// </summary>
        public void ExitCallback(string stateName, UnityAction callback, CallBackControl control)
        {
            if (_states.TryGetValue(stateName, out State state))
            {
                if(control == CallBackControl.Add) state.OnPlayExit += callback;
                else state.OnPlayExit -= callback;
            }
            else Debug.LogWarning("ステート終了時のコールバックに対応していない: " + stateName);
        }

        /// <summary>
        /// アニメーションイベントのコールバックを登録/解除
        /// </summary>
        public void AnimationEventCallback(AnimationEvent.Key key, UnityAction callback, CallBackControl control)
        {
            if (control == CallBackControl.Add) _animationEvent.Register(key, callback);
            else _animationEvent.Release(key, callback);
        }

        /// <summary>
        /// アニメーションのパラメータを設定する。
        /// </summary>
        public void SetParameter(string name, float value)
        {
            _animator.SetFloat(name, value);
        }

        /// <summary>
        /// アニメーションのトリガーを設定する。
        /// </summary>
        public void SetTrigger(string name)
        {
            _animator.SetTrigger(name);
        }

        /// <summary>
        /// 非表示になる際の後始末。
        /// </summary>
        public void Cleaningup()
        {
            // コールバックを登録解除
            foreach (KeyValuePair<string, State> s in _states)
            {
                s.Value.OnPlayEnter = null;
                s.Value.OnPlayExit = null;
            }

            // 警告対策のためAnimatorを無効化
            _animator.enabled = false;
        }
    }
}
