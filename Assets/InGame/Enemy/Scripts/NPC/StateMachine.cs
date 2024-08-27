using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.NPC
{
    public enum StateKey
    {
        Hide,
        Approach,
        Action,
        Escape,
        Delete,
    }

    public class StateMachine
    {
        private Dictionary<StateKey, State<StateKey>> _states;
        private State<StateKey> _currentState;
        private bool _isDisposed;

        public StateMachine(RequiredRef requiredRef)
        {
            Ref = requiredRef;
            Initialize();
        }

        public RequiredRef Ref { get; }

        // 初期化処理。
        private void Initialize()
        {
            // ステートを作成し、辞書で管理。
            _states = Ref.States;
            _states.Add(StateKey.Hide, new HideState(Ref));
            _states.Add(StateKey.Approach, new ApproachState(Ref));
            _states.Add(StateKey.Action, new ActionState(Ref));
            _states.Add(StateKey.Escape, new EscapeState(Ref));
            _states.Add(StateKey.Delete, new DeleteState(Ref));

            // 初期状態では画面に表示しない。
            _currentState = _states[StateKey.Hide];
        }

        /// <summary>
        /// 更新。
        /// </summary>
        public Result Update()
        {
            // ステートマシンを更新。
            _currentState = _currentState.Update();

            bool isComplete = Ref.BlackBoard.CurrentState == StateKey.Delete;
            if (isComplete) return Result.Complete;
            else return Result.Running;
        }

        /// <summary>
        /// 破棄。
        /// アニメーション含むオブジェクトが動かなくなるので、画面から消す直前に呼ぶこと。
        /// </summary>
        public void Dispose()
        {
            // 二度実行するのを防ぐ。
            if (_isDisposed) return;
            else _isDisposed = true;

            // ステートマシンを破棄。
            foreach (KeyValuePair<StateKey, State<StateKey>> s in _states)
            {
                s.Value.Dispose();
            }

            // ステートを破棄した時点でAnimatorに関する操作もこれ以上しない。
            // 警告対策のためAnimatorを無効化しておく。
            Ref.Animator.enabled = false;
        }
    }
}
