namespace Enemy.FSM
{
    /// <summary>
    /// 移動しつつ攻撃するステート。
    /// 接近後、アイドル状態を経由して遷移してくる。
    /// </summary>
    public class BattleByLauncherState : BattleState
    {
        private enum AnimationGroup
        {
            Other,  // 初期状態
            Idle,   // Idle
            Hold,   // HoldStart~HoldLoop
            Fire,   // Fire
            Reload, // Reload
        }

        // 現在のアニメーションのステートによって処理を分岐するために使用する。
        private AnimationGroup _currentAnimGroup;

        public BattleByLauncherState(StateRequiredRef requiredRef) : base(requiredRef)
        {
            // アニメーションのステートの遷移をトリガーする。
            Register(BodyAnimationConst.Launcher.Idle, BodyAnimationConst.Layer.UpperBody, AnimationGroup.Idle);
            Register(BodyAnimationConst.Launcher.HoldStart, BodyAnimationConst.Layer.UpperBody, AnimationGroup.Hold);
            Register(BodyAnimationConst.Launcher.Fire, BodyAnimationConst.Layer.BaseLayer, AnimationGroup.Fire);
            Register(BodyAnimationConst.Launcher.Reload, BodyAnimationConst.Layer.BaseLayer, AnimationGroup.Reload);

            // stateNameのアニメーションのステートに遷移してきたタイミング(Enter)のみトリガーしている。
            // このメソッドで登録していないアニメーションのステートに遷移した場合、
            // _currentAnimGroupの値が元のままになるので注意。
            void Register(string stateName, int layerIndex, AnimationGroup animGroup)
            {
                _animation.RegisterStateEnterCallback(StateKey.Battle, stateName, layerIndex, () => _currentAnimGroup = animGroup);
            }
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
            // 死亡と撤退どちらの場合でも、武器を下ろすアニメーションをトリガー。
            _animation.SetTrigger(BodyAnimationConst.Param.AttackEndTrigger);
        }

        protected override void Stay()
        {
            // 継承元のBattleStateクラス、雑魚敵の共通したメソッド群。
            PlayDamageSE();
            if (BattleExit()) return;
            MoveToSlot(_params.MoveSpeed.Chase);

            // どのアニメーションが再生されているかによって処理を分ける。
            if (_currentAnimGroup == AnimationGroup.Idle) StayIdle();
            else if (_currentAnimGroup == AnimationGroup.Hold) StayHold();
            else if (_currentAnimGroup == AnimationGroup.Fire) StayFire();
            else if (_currentAnimGroup == AnimationGroup.Reload) StayReload();
            else StayOther();
        }

        public override void Dispose()
        {
            // コールバックの登録解除。
            _animation.ReleaseStateCallback(StateKey.Battle);
        }

        // アニメーションがアイドル状態
        private void StayIdle()
        {
            // 攻撃可能な場合は武器構えのアニメーション再生。
            if (IsAttack())
            {
                _animation.SetTrigger(BodyAnimationConst.Param.AttackSetTrigger);
            }
        }

        // アニメーションが武器構え状態
        private void StayHold()
        {
            // 現状、特にプランナーから指示が無いので構え->発射を瞬時に行う。
            _animation.SetTrigger(BodyAnimationConst.Param.AttackTrigger);
        }

        // アニメーションが攻撃状態
        private void StayFire()
        {
            //
        }

        // アニメーションが武器リロード状態
        private void StayReload()
        {
            //
        }

        // アニメーションがそれ以外状態
        private void StayOther()
        {
            //
        }
    }
}