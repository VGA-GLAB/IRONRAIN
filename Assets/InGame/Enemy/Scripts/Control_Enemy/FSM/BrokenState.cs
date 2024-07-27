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
        private AgentScript _agentScript;

        // 画面に表示された状態で死亡したかどうかで演出を再生するかどうかを決める。
        private bool _isRendererEnabledOnEnter;
        // 一度だけアニメーションやエフェクトを再生するためのフラグ
        private bool _isPlaying;

        // 死亡演出が終わるまで待つためのタイマー。
        // 後々はアニメーションの終了にフックするので必要なくなる。
        private float _exitElapsed;

        public BrokenState(EnemyParams enemyParams, BlackBoard blackBoard, Body body, BodyAnimation bodyAnimation, 
            Effector effector, AgentScript agentScript)
        {
            _params = enemyParams;
            _blackBoard = blackBoard;
            _body = body;
            _animation = bodyAnimation;
            _effector = effector;
            _agentScript = agentScript;
        }

        public override StateKey Key => StateKey.Broken;

        protected override void Enter()
        {
            // 画面に表示されていない状態で死亡したかチェック
            _isRendererEnabledOnEnter = _body.IsModelEnabled();

            _isPlaying = false;
            _exitElapsed = 0;

            // レーダーマップから消す。
            if (_agentScript != null) _agentScript.EnemyDestory();
        }

        protected override void Exit()
        {
            // このステートから遷移しないので呼ばれない。
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            // ステートの開始から少し経ったら後始末の準備完了フラグを立てる。
            _exitElapsed += _blackBoard.PausableDeltaTime;
            if (_exitElapsed > _params.Other.BrokenDelay)
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

            // 死亡アニメーションはその瞬間に強制的に遷移させるため、ステートを指定して再生。
            _animation.Play(stateName);

            AudioWrapper.PlaySE("SE_Kill");

            _effector.PlayDestroyedEffect();
            _effector.ThrusterEnable(false);
            _effector.TrailEnable(false);

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
