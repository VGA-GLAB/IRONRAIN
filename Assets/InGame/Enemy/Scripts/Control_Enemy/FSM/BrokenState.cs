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
        private Body _body;
        private BodyAnimation _animation;
        private Effector _effector;

        // 一度だけアニメーションやエフェクトを再生するためのフラグ
        private bool _isPlaying;

        // 死亡演出が終わるまで待つためのタイマー。
        // 後々はアニメーションの終了にフックするので必要なくなる。
        private float _exitElapsed;

        public BrokenState(EnemyParams enemyParams, BlackBoard blackBoard, Body body, BodyAnimation bodyAnimation, 
            Effector effector)
        {
            _params = enemyParams;
            _blackBoard = blackBoard;
            _body = body;
            _animation = bodyAnimation;
            _effector = effector;
        }

        public override StateKey Key => StateKey.Broken;

        protected override void Enter()
        {
            _isPlaying = false;
            _exitElapsed = 0;
        }

        protected override void Exit()
        {
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            // 画面に表示されていない状態で死亡した場合、死亡演出を再生しない。
            if (!_body.IsRendererEnabled()) return;

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
#if false
            _animation.Play(stateName);
#elif true
            _body.RendererEnable(false);
            _effector.Play(EffectKey.Destroyed, _blackBoard);
#endif

            // ダメージの当たり判定を消す。
            _body.HitBoxEnable(false);

            // 死亡演出が終わるまで待つ。
            _exitElapsed += _blackBoard.PausableDeltaTime;
            if (_exitElapsed > 2.0f) // 値は適当。
            {
                _blackBoard.IsExitCompleted = true;
            }

            /* 必要に応じて再生後にアイドル状態に戻るような処理を入れても可。 */
        }
    }
}
