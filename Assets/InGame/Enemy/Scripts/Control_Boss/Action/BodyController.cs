using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Control.FSM;
using Enemy.Control.Boss.FSM;

namespace Enemy.Control.Boss
{
    public class BodyController
    {
        // アニメーションなど、BossControllerのイベント関数外での処理を扱う。
        // そのため、結果を返して完了まで待ってもらう。
        public enum Result { Running, Complete };

        private BlackBoard _blackBoard;

        // ステートベースで制御する。
        private Dictionary<StateKey, State> _stateTable;
        private State _currentState;

        // 既に後始末処理を実行済みかを判定するフラグ。
        private bool _isCleanup;

        public BodyController(Transform transform, BossParams bossParams, BlackBoard blackBoard, Transform offset, 
            Transform rotate, Transform[] models, Animator animator, BossEffects effects, Collider[] hitBoxes, 
            IReadOnlyCollection<FunnelController> funnels)
        {
            _blackBoard = blackBoard;

            // ボス戦開始時にレーダーマップに表示、QTEイベントの最後(死亡演出？)にレーダーマップから消す。
            // それぞれ対応するステートに渡して処理を呼んでもらう。
            if (!transform.TryGetComponent(out AgentScript agentScript))
            {
                Debug.LogWarning($"{nameof(AgentScript)}がアタッチされていないため、レーダーマップに写らない");
            }

            // ボスのオブジェクトの構成が雑魚敵と同じ想定なので流用する。
            Body body = new Body(transform, offset, rotate, models, hitBoxes);
            BodyAnimation bodyAnimation = new BodyAnimation(animator);
            // エフェクトはボス用のもの
            Effector effector = new Effector(effects, blackBoard);
            _stateTable = new Dictionary<StateKey, State>
            {
                { StateKey.Idle, new Boss.FSM.IdleState(blackBoard) },
                { StateKey.Appear, new AppearState(blackBoard, effector) },
                { StateKey.Battle, new BattleState(blackBoard, body, bodyAnimation, funnels, agentScript) },
                { StateKey.QteEvent, new QteEventState(blackBoard, bodyAnimation, effector, agentScript) },
            };

            // 初期状態では画面に表示されている。
            _currentState = _stateTable[StateKey.Idle];
        }

        /// <summary>
        /// 更新。
        /// </summary>
        public Result Update()
        {
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
            foreach (KeyValuePair<StateKey, State> s in _stateTable)
            {
                s.Value.Dispose();
            }
        }
    }
}
