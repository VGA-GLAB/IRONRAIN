namespace Enemy.FSM
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

        // 一度だけアニメーションやエフェクトを再生するためのフラグ
        private bool _isPlaying;
        // 死亡演出が終わるまで待つためのタイマー。
        // 後々はアニメーションの終了にフックするので必要なくなる。
        private float _exitElapsed;

        public BrokenState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            _params = requiredRef.EnemyParams;
            _blackBoard = requiredRef.BlackBoard;
            _body = requiredRef.Body;
            _animation = requiredRef.BodyAnimation;
            _effector = requiredRef.Effector;
            _agentScript = requiredRef.AgentScript;
        }

        protected override void Enter()
        {
            _blackBoard.CurrentState = StateKey.Broken;

            _isPlaying = false;
            _exitElapsed = 0;
            
            // レーダーマップから消す。
            if (_agentScript != null) _agentScript.EnemyDestory();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            // ステートの開始から少し経ったら削除状態に遷移。
            _exitElapsed += _blackBoard.PausableDeltaTime;
            if (_exitElapsed > _params.Other.BrokenDelay)
            {
                TryChangeState(StateKey.Delete);
            }

            // 一度だけ再生すれば良い。
            if (_isPlaying) return;
            _isPlaying = true;

            // 再生するアニメーション名が敵の種類によって違う。
            string stateName = "";
            if (_params.Type == EnemyType.Assault) stateName = BodyAnimationConst.Assault.Damage;
            else if (_params.Type == EnemyType.Launcher) stateName = BodyAnimationConst.Launcher.Damage;
            else if (_params.Type == EnemyType.Shield) stateName = BodyAnimationConst.Shield.Damage;

            // 設定ミスなどで対応しているアニメーションが無い場合。
            if (stateName == "") return;

            // 死亡アニメーションはその瞬間に強制的に遷移させるため、ステートを指定して再生。
            _animation.Play(stateName, BodyAnimationConst.Layer.BaseLayer);

            AudioWrapper.PlaySE("SE_Kill");

            _effector.PlayDestroyedEffect();
            _effector.ThrusterEnable(false);
            _effector.TrailEnable(false);

            // 当たり判定を消す。
            _body.HitBoxEnable(false);
        }
    }
}
