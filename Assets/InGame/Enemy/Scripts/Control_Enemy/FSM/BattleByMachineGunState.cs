using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.FSM
{
    /// <summary>
    /// 移動しつつ攻撃するステート。
    /// 接近後、アイドル状態を経由して遷移してくる。
    /// </summary>
    public class BattleByMachineGunState : BattleState
    {
        private enum AnimationGroup
        {
            Other, // 初期状態
            Idle,  // Idle
            Hold,  // HoldStart~HoldLoop
            Fire,  // FireLoop
        }

        // 現在のアニメーションのステートによって処理を分岐するために使用する。
        private AnimationGroup _currentAnimGroup;

        public BattleByMachineGunState(EnemyParams enemyParams, BlackBoard blackBoard, Body body, BodyAnimation animation)
            : base(enemyParams, blackBoard, body, animation)
        {
            // アニメーションのステートの遷移をトリガーする。
            Register(BodyAnimation.StateName.MachineGun.Idle, AnimationGroup.Idle);
            Register(BodyAnimation.StateName.MachineGun.HoldStart, AnimationGroup.Hold);
            Register(BodyAnimation.StateName.MachineGun.FireLoop, AnimationGroup.Fire);

            // stateNameのアニメーションのステートに遷移してきたタイミング(Enter)のみトリガーしている。
            // このメソッドで登録していないアニメーションのステートに遷移した場合、
            // _currentAnimGroupの値が元のままになるので注意。
            void Register(string stateName, AnimationGroup animGroup)
            {
                _animation.RegisterStateEnterCallback(Key, stateName, () => _currentAnimGroup = animGroup);
            }
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
            // 死亡と撤退どちらの場合でも、武器を下ろすアニメーションをトリガー。
            _animation.SetTrigger(BodyAnimation.ParamName.AttackEndTrigger);
        }

        protected override void Stay(IReadOnlyDictionary<StateKey, State> stateTable)
        {
            // 継承元のBattleStateクラス、雑魚敵の共通したメソッド群。
            PlayDamageSE();
            if (BattleExit(stateTable)) return;
            Move();

            // どのアニメーションが再生されているかによって処理を分ける。
            if (_currentAnimGroup == AnimationGroup.Idle) StayIdle();
            else if (_currentAnimGroup == AnimationGroup.Hold) StayHold();
            else if (_currentAnimGroup == AnimationGroup.Fire) StayFire();
            else StayOther();
        }

        public override void Dispose()
        {
            // コールバックの登録解除。
            _animation.ReleaseStateCallback(Key);
        }

        // アニメーションがアイドル状態
        private void StayIdle()
        {
            // 攻撃可能なタイミングになった場合、攻撃するまで毎フレーム書き込まれる。
            // Brain側はアニメーションの状態を把握していないので、ここで調整する必要がある。
            while (_blackBoard.ActionPlans.TryDequeue(out ActionPlan plan))
            {
                // 攻撃のアニメーション再生をトリガー。
                if (plan.Choice == Choice.Attack)
                {
                    _animation.SetTrigger(BodyAnimation.ParamName.AttackSetTrigger);
                }
            }
        }

        // アニメーションが武器構え状態
        private void StayHold()
        {
            // 現状、特にプランナーから指示が無いので構え->発射を瞬時に行う。
            _animation.SetTrigger(BodyAnimation.ParamName.AttackTrigger);
        }

        // アニメーションが攻撃状態
        private void StayFire()
        {
            // チュートリアル用の敵の場合、攻撃状態になった瞬間に攻撃終了のフラグを立てる。
            // Animatorのenemy_assult_fire_lpステートを繰り返す遷移にHasExitTimeのチェックが入っている前提。
            if (_params.SpecialCondition == SpecialCondition.ManualAttack)
            {
                // この場合、1回攻撃のアニメーションが再生された後、アイドル状態に戻るはず。
                _animation.SetTrigger(BodyAnimation.ParamName.AttackEndTrigger);
            }
        }

        // アニメーションがそれ以外状態
        private void StayOther()
        {
            //
        }
    }
}