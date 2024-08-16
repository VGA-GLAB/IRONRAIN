using Enemy.FSM;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 黒板に書き込まれた内容を基にオブジェクトを制御する。
    /// </summary>
    public class BodyController
    {
        // アニメーションなど、EnemyControllerのイベント関数外での処理を扱う。
        // そのため、結果を返して完了まで待ってもらう。
        public enum Result { Running, Complete };

        private BlackBoard _blackBoard;
        private Animator _animator;

        // ステートベースで制御する。
        private Dictionary<StateKey, State> _states;
        private State _currentState;

        // 既に後始末処理を実行済みかを判定するフラグ。
        private bool _isCleanup;

        public BodyController(RequiredRef requiredRef)
        {
            _blackBoard = requiredRef.BlackBoard;
            _animator = requiredRef.Animator;

            // 各ステートに必要な参照をまとめる。
            StateRequiredRef stateRequiredRef = new StateRequiredRef(
                states: new Dictionary<StateKey, State>(),
                enemyParams: requiredRef.EnemyParams,
                blackBoard: requiredRef.BlackBoard,
                body: new Body(requiredRef),
                bodyAnimation: new BodyAnimation(requiredRef),
                effector: new Effector(requiredRef),
                agentScript: requiredRef.Transform.GetComponent<AgentScript>()
                );

            // ステートを作成し、辞書で管理。
            _states = stateRequiredRef.States;
            _states.Add(StateKey.Approach, new ApproachState(stateRequiredRef));
            _states.Add(StateKey.Broken, new BrokenState(stateRequiredRef));
            _states.Add(StateKey.Escape, new EscapeState(stateRequiredRef));
            _states.Add(StateKey.Hide, new HideState(stateRequiredRef));
            _states.Add(StateKey.Delete, new DeleteState(stateRequiredRef));

            // 戦闘ステートは装備によって違う。
            {
                EnemyType t = requiredRef.EnemyParams.Type;
                BattleState b = null;
                if (t == EnemyType.Assault) b = new BattleByAssaultState(stateRequiredRef);
                if (t == EnemyType.Launcher) b = new BattleByLauncherState(stateRequiredRef);
                if (t == EnemyType.Shield) b = new BattleByShieldState(stateRequiredRef);
                _states.Add(StateKey.Battle, b);
            }

            // 初期状態では画面に表示しない。
            _currentState = _states[StateKey.Hide];
        }

        /// <summary>
        /// 更新。
        /// </summary>
        public Result Update()
        {
            // アニメーション速度はステートに依存しない。
            // ポーズ時にアニメーションが止まる。
            _animator.SetFloat(BodyAnimationConst.Param.PlaySpeed, _blackBoard.PausableTimeScale);
            
            // ステートマシンを更新。
            _currentState = _currentState.Update();

            // ステート内で後始末の準備完了フラグが立った場合は、これ以上更新しなくて良い。
            if (_blackBoard.IsCleanupReady) return Result.Complete;
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
            foreach(KeyValuePair<StateKey, State> s in _states)
            {
                s.Value.Dispose();
            }

            // ステートを破棄した時点でAnimatorに関する操作もこれ以上しない。
            // 警告対策のためAnimatorを無効化しておく。
            _animator.enabled = false;
        }
    }
}
