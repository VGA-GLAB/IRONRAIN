using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss.FSM
{
    /// <summary>
    /// ステートを指定するキー
    /// </summary>
    public enum StateKey
    {
        Base,
        Appear,
        Idle,
        Hide,
        QteEvent,
        Delete,
        BladeAttack,
        LauncherFire,
    }

    /// <summary>
    /// ステートマシンの各種ステートはこのクラスを継承する。
    /// </summary>
    [System.Serializable]
    public abstract class State
    {
        private enum Stage
        {
            Enter,
            Stay,
            Exit,
        }

        private Stage _stage;
        private State _next;
        private IReadOnlyDictionary<StateKey, State> _states;

        public State(IReadOnlyDictionary<StateKey, State> states)
        {
            _states = states;
        }

        /// <summary>
        /// 1度の呼び出しでステートの段階に応じてEnter,Stay,Exitのうちどれか1つが実行される。
        /// 次の呼び出しで実行されるステートを返す。
        /// </summary>
        public State Update()
        {
            Always();

            if (_stage == Stage.Enter)
            {
                Enter();
                _stage = Stage.Stay;
            }
            else if (_stage == Stage.Stay)
            {
                Stay();
            }
            else if (_stage == Stage.Exit)
            {
                Exit();
                _stage = Stage.Enter;

                return _next;
            }

            return this;
        }

        protected abstract void Enter();
        protected abstract void Stay();
        protected abstract void Exit();

        /// <summary>
        /// Enter、Stay、Exit、全てのタイミングで呼び出される。
        /// </summary>
        protected virtual void Always() { }

        /// <summary>
        /// ゲーム終了時にステートを破棄するタイミングで呼ぶ処理。
        /// </summary>
        public virtual void Dispose() { }

        /// <summary>
        /// Enterが呼ばれている状態かつ、ステートの遷移処理を呼んでいない場合のみ遷移可能。
        /// </summary>
        public bool TryChangeState(StateKey next)
        {
            if (_stage == Stage.Enter)
            {
                Debug.LogWarning($"Enterが呼ばれる前にステートを遷移することは不可能。遷移先:{next}");
                return false;
            }
            else if (_stage == Stage.Exit)
            {
                Debug.LogWarning($"既に別のステートに遷移する処理が呼ばれている。遷移先:{next}");
                return false;
            }

            _stage = Stage.Exit;
            _next = _states[next];

            return true;
        }
    }
}
