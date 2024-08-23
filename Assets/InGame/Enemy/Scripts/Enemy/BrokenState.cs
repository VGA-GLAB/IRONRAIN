namespace Enemy
{
    /// <summary>
    /// 撃破されたステート。
    /// </summary>
    public class BrokenState : State<StateKey>
    {
        // 一度だけアニメーションやエフェクトを再生するためのフラグ
        private bool _isPlaying;
        // 死亡演出が終わるまで待つためのタイマー。
        // 後々はアニメーションの終了にフックするので必要なくなる。
        private float _exitElapsed;

        public BrokenState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Broken;

            _isPlaying = false;
            _exitElapsed = 0;

            // レーダーマップから消す。
            AgentScript agent = Ref.AgentScript;
            if (agent != null) agent.EnemyDestory();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            // ステートの開始から少し経ったら削除状態に遷移。
            float dt = Ref.BlackBoard.PausableDeltaTime;
            float delay = Ref.EnemyParams.Other.BrokenDelay;
            _exitElapsed += dt;
            if (_exitElapsed > delay) TryChangeState(StateKey.Delete);

            // 一度だけ再生すれば良い。
            if (_isPlaying) return;
            _isPlaying = true;

            // 再生するアニメーション名が敵の種類によって違う。
            EnemyType type = Ref.EnemyParams.Type;
            string stateName = "";
            if (type == EnemyType.Assault) stateName = BodyAnimationConst.Assault.Damage;
            else if (type == EnemyType.Launcher) stateName = BodyAnimationConst.Launcher.Damage;
            else if (type == EnemyType.Shield) stateName = BodyAnimationConst.Shield.Damage;

            // 設定ミスなどで対応しているアニメーションが無い場合。
            if (stateName == "") return;

            // 死亡アニメーションはその瞬間に強制的に遷移させるため、ステートを指定して再生。
            int layer = BodyAnimationConst.Layer.BaseLayer;
            Ref.BodyAnimation.Play(stateName, layer);

            AudioWrapper.PlaySE("SE_Kill");

            Ref.Effector.PlayDestroyedEffect();
            Ref.Effector.ThrusterEnable(false);
            Ref.Effector.TrailEnable(false);

            // 当たり判定を消す。
            Ref.Body.HitBoxEnable(false);
        }
    }
}
