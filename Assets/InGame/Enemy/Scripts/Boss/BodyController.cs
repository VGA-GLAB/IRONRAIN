using Enemy.Boss.FSM;
using System.Collections.Generic;

namespace Enemy.Boss
{
    public class BodyController
    {
        // アニメーションなど、BossControllerのイベント関数外での処理を扱う。
        // そのため、結果を返して完了まで待ってもらう。
        public enum Result { Running, Complete };

        private BlackBoard _blackBoard;

        // ステートベースで制御する。
        private Dictionary<StateKey, State> _states;
        private State _currentState;

        // 既に後始末処理を実行済みかを判定するフラグ。
        private bool _isCleanup;

        public BodyController(RequiredRef requiredRef)
        {
            _blackBoard = requiredRef.BlackBoard;

            _states = requiredRef.States;
            _states.Add(StateKey.Appear, new AppearState(requiredRef));
            _states.Add(StateKey.Idle, new IdleState(requiredRef));
            _states.Add(StateKey.BladeAttack, new BladeAttackState(requiredRef));
            _states.Add(StateKey.LauncherFire, new LauncherFireState(requiredRef));
            _states.Add(StateKey.QteEvent, new QteEventState(requiredRef));
            _states.Add(StateKey.Hide, new HideState(requiredRef));
            _states.Add(StateKey.FunnelExpand, new FunnelExpandState(requiredRef));
            _states.Add(StateKey.FunnelAttack, new FunnelAttackState(requiredRef));

            // 初期状態
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
            if (_isCleanup) return;
            else _isCleanup = true;

            // ステートマシンを破棄。
            foreach (KeyValuePair<StateKey, State> s in _states)
            {
                s.Value.Dispose();
            }
        }
    }
}
