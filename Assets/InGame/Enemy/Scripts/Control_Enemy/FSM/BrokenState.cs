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

        // 画面に表示された状態で死亡したかどうかで演出を再生するかどうかを決める。
        private bool _isRendererEnabledOnEnter;
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
            // 画面に表示されていない状態で死亡したかチェック
            _isRendererEnabledOnEnter = _body.IsModelEnabled();

            _isPlaying = false;
            _exitElapsed = 0;
        }

        protected override void Exit()
        {
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            // ステートの開始から少し経ったら後始末の準備完了フラグを立てる。
            _exitElapsed += _blackBoard.PausableDeltaTime;
            if (_exitElapsed > 1.5f) // 値は適当。
            {
                _blackBoard.IsCleanupReady = true;
            }

            // Enterのタイミングで画面に表示されていたかで処理を分ける。
            if (_isRendererEnabledOnEnter) StayRendererEnabledOnEnter();
            else StayRendererDisabledOnEnter();
        }

        // Enterのタイミングで画面に表示されていた場合。
        private void StayRendererEnabledOnEnter()
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
#if false
            _animation.Play(stateName);
#elif true
            _body.ModelEnable(false);
            _effector.Play(EffectKey.Destroyed, _blackBoard);
#endif

            // 当たり判定を消す。
            _body.HitBoxEnable(false);
        }

        // Enterのタイミングで画面に表示されていなかった場合。
        private void StayRendererDisabledOnEnter()
        {
            // 即座に後始末の準備完了フラグを立てる。
            _blackBoard.IsCleanupReady = true;
        }
    }
}
