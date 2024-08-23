namespace Enemy
{
    /// <summary>
    /// 移動しつつ攻撃するステート。
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

        private LauncherEquipment _equipment;

        // 現在のアニメーションのステートによって処理を分岐するために使用する。
        private AnimationGroup _currentAnimGroup;

        public BattleByLauncherState(RequiredRef requiredRef) : base(requiredRef)
        {
            _equipment = requiredRef.Equipment as LauncherEquipment;
            _equipment.OnShootAction += OnShoot;

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
                Ref.BodyAnimation.RegisterStateEnterCallback(
                    nameof(BattleByLauncherState), 
                    stateName, 
                    layerIndex, 
                    () => _currentAnimGroup = animGroup
                    );
            }
        }

        private void OnShoot()
        {
            AttackTrigger();
        }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Battle;
        }

        protected override void Exit()
        {
            // 死亡と撤退どちらの場合でも、武器を下ろすアニメーションをトリガー。
            Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.AttackEnd);
        }

        protected override void StayIfBattle()
        {
            // どのアニメーションが再生されているかによって処理を分ける。
            if (_currentAnimGroup == AnimationGroup.Idle) StayIdle();
            else if (_currentAnimGroup == AnimationGroup.Hold) StayHold();
            else if (_currentAnimGroup == AnimationGroup.Fire) StayFire();
            else if (_currentAnimGroup == AnimationGroup.Reload) StayReload();
            else StayOther();
        }

        public override void Dispose()
        {
            _equipment.OnShootAction -= OnShoot;

            // コールバックの登録解除。
            Ref.BodyAnimation.ReleaseStateCallback(nameof(BattleByLauncherState));
        }

        // アニメーションがアイドル状態
        private void StayIdle()
        {
            Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.AttackSet);
        }

        // アニメーションが武器構え状態
        private void StayHold()
        {
            if (IsAttack())
            {
                Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.Attack);
                AttackTrigger();
            }
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