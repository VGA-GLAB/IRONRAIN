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

            // 各ステートに必要な参照をまとめる。
            StateRequiredRef stateRequiredRef = new StateRequiredRef(
                states: new Dictionary<StateKey, State>(),
                bossParams: requiredRef.BossParams,
                blackBoard: requiredRef.BlackBoard,
                body: new Body(requiredRef),
                bodyAnimation: new BodyAnimation(requiredRef),
                effector: new Effector(requiredRef),
                funnels: requiredRef.Funnels,
                agentScript: requiredRef.Transform.GetComponent<AgentScript>()
                );

            _states = stateRequiredRef.States;
            _states.Add(StateKey.Appear, new AppearState(stateRequiredRef));
            _states.Add(StateKey.Battle, new BattleState(stateRequiredRef));
            _states.Add(StateKey.QteEvent, new QteEventState(stateRequiredRef));
            _states.Add(StateKey.Hide, new HideState(stateRequiredRef));

            // 初期状態では画面に表示されている。
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
