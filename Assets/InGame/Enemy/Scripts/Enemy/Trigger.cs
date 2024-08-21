using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Perception層とAction層が互いに書き込む。
    /// 命令されたら1度だけ実行し、実行したことを命令する側で検知する。
    /// </summary>
    public struct Trigger
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
        /// 命令まだの場合は、一度だけ命令し、値を命令済みに変更。
        /// </summary>
        public bool Order()
        {
            if (!IsOrdered())
            {
                _state = State.Ordered;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// 命令済みかどうかを返す。
        /// </summary>
        public bool IsOrdered() => _state == State.Ordered;

        /// <summary>
        /// 命令されている場合は、一度だけ実行し、値を実行済みに変更。
        /// </summary>
        public bool Execute()
        {
            if (_state == State.Ordered)
            {
                _state = State.Executed;
                return true;
            }
            else return false;
        }
    }
}
