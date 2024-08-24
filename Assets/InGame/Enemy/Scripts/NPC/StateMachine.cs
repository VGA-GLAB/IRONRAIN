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
        // アニメーションなど、EnemyControllerのイベント関数外での処理を扱う。
        // そのため、結果を返して完了まで待ってもらう。
        public enum Result { Running, Complete };

        // ステートベースで制御する。
        private Dictionary<StateKey, State<StateKey>> _states;
        private State<StateKey> _currentState;

        // 既に後始末処理を実行済みかを判定するフラグ。
        private bool _isCleanup;

        public StateMachine(RequiredRef requiredRef)
        {
            Ref = requiredRef;

            // ステートを作成し、辞書で管理。
            _states = requiredRef.States;
            _states.Add(StateKey.Hide, new HideState(requiredRef));
            _states.Add(StateKey.Approach, new ApproachState(requiredRef));
            _states.Add(StateKey.Action, new ActionState(requiredRef));
            _states.Add(StateKey.Escape, new EscapeState(requiredRef));
            _states.Add(StateKey.Delete, new DeleteState(requiredRef));

            // 初期状態では画面に表示しない。
            _currentState = _states[StateKey.Hide];
        }

        public RequiredRef Ref { get; }

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
            if (_isCleanup) return;
            else _isCleanup = true;

            // ステートマシンを破棄。
            foreach (KeyValuePair<StateKey, State<StateKey>> s in _states)
            {
                s.Value.Dispose();
            }
        }
    }
}
