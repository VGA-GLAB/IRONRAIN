using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Perception層とAction層が互いに書き込む。
    /// 命令されたら1度だけ実行し、実行したことを命令する側で検知する。
    /// </summary>
    public class Trigger
    {
        private enum State
        {
            None,    // 命令まだ
            Ordered, // 命令済み
            Executed // 実行済み
        }
        
        // 現状、命令まだの状態になるのが、一度も命令していない最初だけ。
        private State _state;

        /// <summary>
        /// 命令済みに変更。
        /// </summary>
        public void Order() => _state = State.Ordered;

        /// <summary>
        /// 命令済みで実行待ちの状態かどうかを返す。
        /// </summary>
        public bool IsWaitingExecute() => _state == State.Ordered;

        /// <summary>
        /// 実行済みかどうかを返す。
        /// </summary>
        public bool IsExecuted() => _state == State.Executed;

        /// <summary>
        /// 実行済みに変更。
        /// </summary>
        public void Execute() => _state = State.Executed;
    }
}
