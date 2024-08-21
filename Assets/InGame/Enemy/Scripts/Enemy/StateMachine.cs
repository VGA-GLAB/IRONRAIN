using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public enum StateKey
    {
        Base,
        Approach,
        Battle,
        Broken,
        Escape,
        Hide,
        Delete,
    }

    public class StateMachine
    {
        // アニメーションなど、EnemyControllerのイベント関数外での処理を扱う。
        // そのため、結果を返して完了まで待ってもらう。
        public enum Result { Running, Complete };

        private BlackBoard _blackBoard;
        private Animator _animator;

        // ステートベースで制御する。
        private Dictionary<StateKey, State<StateKey>> _states;
        private State<StateKey> _currentState;

        // 既に後始末処理を実行済みかを判定するフラグ。
        private bool _isCleanup;

        public StateMachine(RequiredRef requiredRef)
        {
            _blackBoard = requiredRef.BlackBoard;
            _animator = requiredRef.Animator;

            // ステートを作成し、辞書で管理。
            _states = requiredRef.States;
            _states.Add(StateKey.Approach, new ApproachState(requiredRef));
            _states.Add(StateKey.Broken, new BrokenState(requiredRef));
            _states.Add(StateKey.Escape, new EscapeState(requiredRef));
            _states.Add(StateKey.Hide, new HideState(requiredRef));
            _states.Add(StateKey.Delete, new DeleteState(requiredRef));

            // 戦闘ステートは装備によって違う。
            {
                EnemyType t = requiredRef.EnemyParams.Type;
                BattleState b = null;
                if (t == EnemyType.Assault) b = new BattleByAssaultState(requiredRef);
                if (t == EnemyType.Launcher) b = new BattleByLauncherState(requiredRef);
                if (t == EnemyType.Shield) b = new BattleByShieldState(requiredRef);
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
            foreach(KeyValuePair<StateKey, State<StateKey>> s in _states)
            {
                s.Value.Dispose();
            }

            // ステートを破棄した時点でAnimatorに関する操作もこれ以上しない。
            // 警告対策のためAnimatorを無効化しておく。
            _animator.enabled = false;
        }
    }
}
