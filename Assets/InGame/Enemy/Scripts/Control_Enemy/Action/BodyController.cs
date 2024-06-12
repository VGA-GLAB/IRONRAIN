﻿using Enemy.Control.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
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
        private Dictionary<StateKey, State> _stateTable;
        private State _currentState;

        // 既に後始末処理を実行済みかを判定するフラグ。
        private bool _isCleanup;

        public BodyController(Transform transform, EnemyParams enemyParams, BlackBoard blackBoard, 
            Transform offset, Transform rotate, Renderer[] renderers, Animator animator, Effect[] effects)
        {
            _blackBoard = blackBoard;
            _animator = animator;

            // 各ステートにTransformやアニメーション、演出を操作するクラスを渡す。
            Body body = new Body(transform, offset, rotate, renderers);
            BodyAnimation bodyAnimation = new BodyAnimation(animator);
            Effector effector = new Effector(effects);
            _stateTable = new Dictionary<StateKey, State>
            {
                { StateKey.Approach, new ApproachState(blackBoard, body, bodyAnimation) },
                { StateKey.Broken, new BrokenState(enemyParams, blackBoard, body, bodyAnimation, effector) },
                { StateKey.Escape, new EscapeState(blackBoard, body, bodyAnimation) },
                { StateKey.Idle, new IdleState(blackBoard) },
                { StateKey.Hide, new HideState(blackBoard, body) },
            };

            // 戦闘ステートは装備によって違う。
            State battleState = null;
            if (enemyParams.Type == EnemyType.MachineGun)
            {
                battleState = new BattleByMachineGunState(blackBoard, body, bodyAnimation);
            }
            else if (enemyParams.Type == EnemyType.Launcher)
            {
                battleState = new BattleByLauncherState(blackBoard, body, bodyAnimation);
            }
            else if (enemyParams.Type == EnemyType.Shield)
            {
                battleState = new BattleByShieldState(blackBoard, body, bodyAnimation);
            }
            _stateTable.Add(StateKey.Battle, battleState);

            // 初期状態では画面に表示されている。
            _currentState = _stateTable[StateKey.Idle];
        }

        /// <summary>
        /// 更新。
        /// </summary>
        public Result Update()
        {
            // アニメーション速度はステートに依存しない。
            // ポーズ時にアニメーションが止まる。
            _animator.SetFloat(BodyAnimation.ParamName.PlaySpeed, _blackBoard.PausableTimeScale);
            
            // ステートマシンを更新。
            _currentState = _currentState.Update(_stateTable);

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
            foreach(KeyValuePair<StateKey, State> s in _stateTable)
            {
                s.Value.Dispose();
            }

            // ステートを破棄した時点でAnimatorに関する操作もこれ以上しない。
            // 警告対策のためAnimatorを無効化しておく。
            _animator.enabled = false;
        }
    }
}
