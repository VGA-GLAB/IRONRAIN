﻿namespace Enemy.FSM
{
    /// <summary>
    /// 移動しつつ攻撃するステート。
    /// 接近後、アイドル状態を経由して遷移してくる。
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
                _animation.RegisterStateEnterCallback(nameof(BattleByLauncherState), stateName, layerIndex, () => _currentAnimGroup = animGroup);
            }
        }

        private void OnShoot()
        {
            // 弾を発射したタイミングで攻撃を実行したことを黒板に書き込む。
            AttackTrigger();
        }

        protected override void Enter()
        {
            _blackBoard.CurrentState = StateKey.Battle;
        }

        protected override void Exit()
        {
            // 死亡と撤退どちらの場合でも、武器を下ろすアニメーションをトリガー。
            _animation.SetTrigger(BodyAnimationConst.Param.AttackEnd);
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
            else StayOther();
        }

        public override void Dispose()
        {
            _equipment.OnShootAction -= OnShoot;

            // コールバックの登録解除。
            _animation.ReleaseStateCallback(nameof(BattleByLauncherState));
        }

        // アニメーションがアイドル状態
        private void StayIdle()
        {
            // 攻撃可能な場合は武器構えのアニメーション再生。
            if (IsAttack())
            {
                _animation.SetTrigger(BodyAnimationConst.Param.AttackSet);
            }
        }

        // アニメーションが武器構え状態
        private void StayHold()
        {
            // 現状、特にプランナーから指示が無いので構え->発射を瞬時に行う。
            _animation.SetTrigger(BodyAnimationConst.Param.Attack);
        }

        // アニメーションが攻撃状態
        private void StayFire()
        {
            // チュートリアル用の敵の場合、攻撃状態になった瞬間に攻撃終了のフラグを立てる。
            // Animatorのenemy_assult_fire_lpステートを繰り返す遷移にHasExitTimeのチェックが入っている前提。
            if (_params.SpecialCondition == SpecialCondition.ManualAttack)
            {
                // この場合、1回攻撃のアニメーションが再生された後、アイドル状態に戻るはず。
                _animation.SetTrigger(BodyAnimationConst.Param.AttackEnd);
            }
        }

        // アニメーションがそれ以外状態
        private void StayOther()
        {
            //
        }
    }
}