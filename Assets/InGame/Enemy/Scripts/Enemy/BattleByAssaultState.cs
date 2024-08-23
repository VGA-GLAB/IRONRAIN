namespace Enemy
{
    /// <summary>
    /// 移動しつつ攻撃するステート。
    /// </summary>
    public class BattleByAssaultState : BattleState
    {
        private enum AnimationGroup
        {
            Other, // 初期状態
            Idle,  // Idle
            Hold,  // HoldStart~HoldLoop
            Fire,  // FireLoop
        }

        private AssaultEquipment _equipment;

        // 現在のアニメーションのステートによって処理を分岐するために使用する。
        private AnimationGroup _currentAnimGroup;

        public BattleByAssaultState(RequiredRef requiredRef) : base(requiredRef)
        {
            _equipment = requiredRef.Equipment as AssaultEquipment;
            _equipment.OnShootAction += OnShoot;

            // アニメーションのステートの遷移をトリガーする。
            Register(BodyAnimationConst.Assault.Idle, BodyAnimationConst.Layer.UpperBody, AnimationGroup.Idle);
            Register(BodyAnimationConst.Assault.HoldStart, BodyAnimationConst.Layer.UpperBody, AnimationGroup.Hold);
            Register(BodyAnimationConst.Assault.FireLoop, BodyAnimationConst.Layer.UpperBody, AnimationGroup.Fire);

            // stateNameのアニメーションのステートに遷移してきたタイミング(Enter)のみトリガーしている。
            // このメソッドで登録していないアニメーションのステートに遷移した場合、
            // _currentAnimGroupの値が元のままになるので注意。
            void Register(string stateName, int layerIndex, AnimationGroup animGroup)
            {
                Ref.BodyAnimation.RegisterStateEnterCallback(
                    nameof(BattleByAssaultState), 
                    stateName, 
                    layerIndex, 
                    () => _currentAnimGroup = animGroup
                    );
            }
        }

        private void OnShoot()
        {
            // 弾を発射したタイミングで攻撃を実行したことを黒板に書き込む。
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
            else StayOther();
        }

        public override void Dispose()
        {
            _equipment.OnShootAction -= OnShoot;

            // コールバックの登録解除。
            Ref.BodyAnimation.ReleaseStateCallback(nameof(BattleByAssaultState));
        }

        // アニメーションがアイドル状態
        private void StayIdle()
        {
            // 攻撃可能な場合は武器構えのアニメーション再生。
            if (IsAttack())
            {
                Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.AttackSet);
            }
        }

        // アニメーションが武器構え状態
        private void StayHold()
        {
            // 現状、特にプランナーから指示が無いので構え->発射を瞬時に行う。
            Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.Attack);
        }

        // アニメーションが攻撃状態
        private void StayFire()
        {
            // チュートリアル用の敵の場合、攻撃状態になった瞬間に攻撃終了のフラグを立てる。
            // Animatorのenemy_assult_fire_lpステートを繰り返す遷移にHasExitTimeのチェックが入っている前提。
            if (Ref.EnemyParams.SpecialCondition == SpecialCondition.ManualAttack)
            {
                // この場合、1回攻撃のアニメーションが再生された後、アイドル状態に戻るはず。
                Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.AttackEnd);
            }
        }

        // アニメーションがそれ以外状態
        private void StayOther()
        {
            //
        }
    }
}