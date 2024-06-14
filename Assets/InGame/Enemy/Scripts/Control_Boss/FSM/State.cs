﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss.FSM
{
    /// <summary>
    /// ステートを指定するキー
    /// </summary>
    public enum StateKey
    {
        Base,
        Idle,     // 初期状態
        Appear,   // 登場演出
        Battle,   // 戦闘
        QteEvent, // プレイヤーの左腕破壊~QTE2回目
        Broken,   // 戦闘終了
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

        /// <summary>
        /// 外部からどのステートなのかを判定するために使用。
        /// </summary>
        public abstract StateKey Key { get; }

        /// <summary>
        /// 1度の呼び出しでステートの段階に応じてEnter,Stay,Exitのうちどれか1つが実行される。
        /// 次の呼び出しで実行されるステートを返す。
        /// </summary>
        public State Update(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            if (_stage == Stage.Enter)
            {
                Enter();
                _stage = Stage.Stay;
            }
            else if (_stage == Stage.Stay)
            {
                Stay(stateTable);
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
        protected abstract void Stay(IReadOnlyDictionary<StateKey, State> stateTable);
        protected abstract void Exit();

        /// <summary>
        /// ゲーム終了時にステートを破棄するタイミングで呼ぶ処理。
        /// </summary>
        public virtual void Dispose() { }

        /// <summary>
        /// Enter()が呼ばれてかつ、ステートの遷移処理を呼んでいない場合のみ遷移可能。
        /// </summary>
        public bool TryChangeState(State next)
        {
            if (_stage == Stage.Enter)
            {
                Debug.LogWarning($"Enterが呼ばれる前にステートを遷移することは不可能: {Key} 遷移先: {next}");
                return false;
            }
            else if (_stage == Stage.Exit)
            {
                Debug.LogWarning($"既に別のステートに遷移する処理が呼ばれている: {Key} 遷移先: {next}");
                return false;
            }

            _stage = Stage.Exit;
            _next = next;

            return true;
        }
    }
}
