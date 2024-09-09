using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    // ObservableStateMachineTriggerコンポーネントがAnimatorControllerの各レイヤーにアタッチされている必要がある。
    // ObservableStateMachineTriggerのコールバックは遷移元Exit→遷移先Enterの順番で呼ばれない。
    // 遷移先Enterの数フレーム後に遷移元Exitが呼ばれる。滑らかなアニメーションの遷移をするため？
    // Layer名からclip名を取得して再生中か調べる方法は、clip名をステート名と揃えるか、
    // clip名の定数のクラスを作る必要があり、これ以上複雑化して管理できなくなりそうなので断念。
    public partial class BodyAnimation
    {
        // Animatorの各ステート
        private class State
        {
            // 再生開始時のコールバック
            public UnityAction OnPlayEnter;
            // 再生終了時のコールバック
            public UnityAction OnPlayExit;

            public State(string s, int layerIndex)
            {
                Hash = Animator.StringToHash(s);
                LayerIndex = layerIndex;
                OnPlayEnter = null;
                OnPlayExit = null;
            }

            public int Hash { get; private set; }
            public int LayerIndex { get; private set; }
        }

        private Animator _animator;
        // 文字列の定数値で各ステートを管理する。
        private List<Dictionary<string, State>> _stateTable;
        // コールバックの解除を行うために、登録した匿名関数を保持しておく。
        // 身体側のステート毎に登録解除出来る。
        private Dictionary<string, List<UnityAction>> _callbacks;

        public BodyAnimation(RequiredRef requiredRef)
        {
            _animator = requiredRef.Animator;
            StateMachineTriggerCallback(_animator.gameObject);
        }

        public BodyAnimation(Boss.RequiredRef requiredRef)
        {
            _animator = requiredRef.Animator;
            StateMachineTriggerCallback(_animator.gameObject);
        }

        public BodyAnimation(Funnel.RequiredRef requiredRef)
        {
            _animator = requiredRef.Animator;
            StateMachineTriggerCallback(_animator.gameObject);
        }

        public BodyAnimation(NPC.RequiredRef requiredRef)
        {
            _animator = requiredRef.Animator;
            StateMachineTriggerCallback(_animator.gameObject);
        }

        // コールバックを登録しているステートをトリガー出来るように設定する。
        private void StateMachineTriggerCallback(GameObject owner)
        {
            _stateTable = new List<Dictionary<string, State>>();
            _callbacks = new Dictionary<string, List<UnityAction>>();

            // AnimatorControllerにアタッチされている数だけ辞書を作り、レイヤーごとに管理。
            ObservableStateMachineTrigger[] triggers = _animator.GetBehaviours<ObservableStateMachineTrigger>();
            for (int i = 0; i < triggers.Length; i++) _stateTable.Add(new Dictionary<string, State>());
            
            // レイヤーごとにコールバックを登録。
            foreach (ObservableStateMachineTrigger t in triggers)
            {
                // ステート開始のタイミングをトリガー。
                t.OnStateEnterAsObservable().Subscribe(a => OnStateEnter(a.StateInfo, a.LayerIndex)).AddTo(owner);
                // ステート終了のタイミングをトリガー。
                t.OnStateExitAsObservable().Subscribe(a => OnStateExit(a.StateInfo, a.LayerIndex)).AddTo(owner);
            }

            // ハッシュ値でどのステートかを判定し、ステート開始のコールバックを呼ぶ。
            void OnStateEnter(AnimatorStateInfo info, int layerIndex)
            {
                foreach (KeyValuePair<string, State> p in _stateTable[layerIndex])
                {
                    if (p.Value.Hash == info.shortNameHash) { p.Value.OnPlayEnter?.Invoke(); return; }
                }
            }

            // ハッシュ値でどのステートかを判定し、ステート終了のコールバックを呼ぶ。
            void OnStateExit(AnimatorStateInfo info, int layerIndex)
            {
                foreach (KeyValuePair<string, State> p in _stateTable[layerIndex])
                {
                    if (p.Value.Hash == info.shortNameHash) { p.Value.OnPlayExit?.Invoke(); return; }
                }
            }
        }

        // 辞書から指定したステートを取得。
        // 登録されていなければ、新しく登録して返す。
        private State GetState(string stateName, int layerIndex)
        {
            if (_stateTable[layerIndex].TryGetValue(stateName, out State state))
            {
                return state;
            }
            else
            {
                _stateTable[layerIndex].Add(stateName, new State(stateName, layerIndex));
                return _stateTable[layerIndex][stateName];
            }
        }

        // コールバックを身体側のステート毎に保持しておき、一括で登録解除できるようにする。
        // ステートマシンの破棄時に身体側のステートごとに一括解除するため、EnterかExitかの区別はしない。
        private void AddCallback(string key, UnityAction callback)
        {
            if (!_callbacks.ContainsKey(key))
            {
                _callbacks.Add(key, new List<UnityAction>());
            }

            _callbacks[key].Add(callback);
        }

        /// <summary>
        /// ステートを指定してアニメーションを再生。
        /// </summary>
        public void Play(string stateName, int layerIndex)
        {
            _animator.Play(GetState(stateName, layerIndex).Hash);
        }

        /// <summary>
        /// ステート開始時のコールバックを登録。
        /// </summary>
        public void RegisterStateEnterCallback(string key, string stateName, int layerIndex, UnityAction callback)
        {
            GetState(stateName, layerIndex).OnPlayEnter += callback;
            AddCallback(key, callback);
        }

        /// <summary>
        /// ステート終了時のコールバックを登録。
        /// </summary>
        public void RegisterStateExitCallback(string key, string stateName, int layerIndex, UnityAction callback)
        {
            GetState(stateName, layerIndex).OnPlayExit += callback;
            AddCallback(key, callback);
        }

        /// <summary>
        /// ステートの遷移に登録したコールバックを解除する。
        /// EnterとExitの区別はせず、身体側のステート毎に一括で解除する。
        /// </summary>
        public void ReleaseStateCallback(string key)
        {
            foreach (Dictionary<string, State> t in _stateTable)
            {
                // どのステートに対してコールバックを登録したかは保持していない。
                // 全てのステートに対して総当たりで解除を試みる。
                foreach (State s in t.Values)
                {
                    foreach (UnityAction a in _callbacks[key])
                    {
                        if (s.OnPlayEnter != null) { s.OnPlayEnter -= a; }
                        if (s.OnPlayExit != null) { s.OnPlayExit -= a; }
                    }
                }
            }
        }

        /// <summary>
        /// アニメーションのfloat型パラメータを設定する。
        /// </summary>
        public void SetFloat(string name, float value) => _animator.SetFloat(name, value);

        /// <summary>
        /// アニメーションのbool型パラメータを設定する。
        /// </summary>
        public void SetBool(string name, bool value) => _animator.SetBool(name, value);

        /// <summary>
        /// アニメーションのトリガーを設定する。
        /// </summary>
        public void SetTrigger(string name) => _animator.SetTrigger(name);

        /// <summary>
        /// アニメーションのトリガーをリセットする。
        /// </summary>
        public void ResetTrigger(string name) => _animator.ResetTrigger(name);

        /// <summary>
        /// アニメーションのfloat型パラメータを取得する。
        /// </summary>
        public float GetFloat(string name) => _animator.GetFloat(name);

        /// <summary>
        /// UpperBodyのWeightをセットする。
        /// </summary>
        public void SetUpperBodyWeight(float value)
        {
            _animator.SetLayerWeight(Const.Layer.UpperBody, value);
        }
    }
}