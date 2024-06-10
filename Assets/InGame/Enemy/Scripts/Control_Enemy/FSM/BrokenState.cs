using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// 撃破されたステート。
    /// </summary>
    public class BrokenState : State
    {
        private EnemyParams _params;
        private BlackBoard _blackBoard;
        private BodyAnimation _animation;
        private Effector _effector;

        // 一度だけアニメーションやエフェクトを再生するためのフラグ
        private bool _isPlaying;

        public BrokenState(EnemyParams enemyParams, BlackBoard blackBoard, BodyAnimation bodyAnimation, 
            Effector effector)
        {
            _params = enemyParams;
            _blackBoard = blackBoard;
            _animation = bodyAnimation;
            _effector = effector;
        }

        public override StateKey Key => StateKey.Broken;

        protected override void Enter()
        {
            _isPlaying = false;
        }

        protected override void Exit()
        {
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            // 一度だけ再生すれば良い。
            if (_isPlaying) return;
            _isPlaying = true;

            // 再生するアニメーション名が敵の種類によって違う。
            string stateName = "";
            if (_params.Type == EnemyType.MachineGun) stateName = BodyAnimation.StateName.MachineGun.Damage;
            else if (_params.Type == EnemyType.Launcher) stateName = BodyAnimation.StateName.Launcher.Damage;
            else if (_params.Type == EnemyType.Shield) stateName = BodyAnimation.StateName.Shield.Damage;

            // 設定ミスなどで対応しているアニメーションが無い場合。
            if (stateName == "") return;

            // 死亡アニメーションとエフェクトを再生。
            _animation.Play(stateName);
            _effector.Play(EffectKey.Destroyed, _blackBoard);

            /* 必要に応じて再生後にアイドル状態に戻るような処理を入れても可。 */
        }
    }
}
