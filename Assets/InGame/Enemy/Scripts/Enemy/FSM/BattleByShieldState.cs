﻿namespace Enemy.FSM
{
    /// <summary>
    /// 移動しつつ攻撃するステート。
    /// 接近後、アイドル状態を経由して遷移してくる。
    /// </summary>
    public class BattleByShieldState : BattleState
    {
        private enum AnimationGroup
        {
            Other,  // 初期状態
            Idle,   // Idle
            Shield, // ShieldStart~ShieldLoop
            Attack, // Attack
        }

        // 現在のアニメーションのステートによって処理を分岐するために使用する。
        private AnimationGroup _currentAnimGroup;

        public BattleByShieldState(StateRequiredRef requiredRef) : base(requiredRef)
        {
            // アニメーションのステートの遷移をトリガーする。
            Register(BodyAnimationConst.Shield.Idle, BodyAnimationConst.Layer.UpperBody, AnimationGroup.Idle);
            Register(BodyAnimationConst.Shield.ShieldStart, BodyAnimationConst.Layer.UpperBody, AnimationGroup.Shield);
            Register(BodyAnimationConst.Shield.Attack, BodyAnimationConst.Layer.UpperBody, AnimationGroup.Attack);

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
            else if (_currentAnimGroup == AnimationGroup.Shield) StayShield();
            else if (_currentAnimGroup == AnimationGroup.Attack) StayAttack();
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

        // アニメーションが盾構え状態
        private void StayShield()
        {
            // 現状、チュートリアルでしか出番が無いので、構え->攻撃を行わずに
            // 構えで止めておくことで、QTETutorialが必ず成功する。
            //_animation.SetTrigger(BodyAnimation.ParamName.AttackTrigger);
        }

        // アニメーションが攻撃状態
        private void StayAttack()
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