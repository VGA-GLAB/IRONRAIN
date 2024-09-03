using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss
{
    public enum StateKey
    {
        Base,
        Appear,
        Idle,
        Hide,
        QteEvent,
        Defeated,
        BladeAttack,
        LauncherFire,
        LaneChange,
        FunnelExpand,
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

        private RequiredRef Ref { get; }

        // 初期化処理。
        private void Initialize()
        {
            _states = Ref.States;
            _states.Add(StateKey.Appear, new AppearState(Ref));
            _states.Add(StateKey.Idle, new IdleState(Ref));
            _states.Add(StateKey.BladeAttack, new BladeAttackState(Ref));
            _states.Add(StateKey.LauncherFire, new LauncherFireState(Ref));
            _states.Add(StateKey.QteEvent, new QteEventState(Ref));
            _states.Add(StateKey.Hide, new HideState(Ref));
            _states.Add(StateKey.FunnelExpand, new FunnelExpandState(Ref));
            _states.Add(StateKey.LaneChange, new LaneChangeState(Ref));
            _states.Add(StateKey.Defeated, new DefeatedState(Ref));

            // 初期状態。
            _currentState = _states[StateKey.Hide];
        }

        /// <summary>
        /// 更新。
        /// </summary>
        public Result Update()
        {
            // ステートマシンを更新。
            _currentState = _currentState.Update();

            return Result.Running; // <- 必要に応じて修正する。
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
        }
    }
}
