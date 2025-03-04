﻿using UnityEngine;
using Enemy.Shield;

namespace Enemy
{
    /// <summary>
    /// 移動しつつ攻撃するステート。
    /// </summary>
    public class BattleByShieldState : BattleState
    {
        // 構える。
        private EnemyActionStep[] _steps;
        private BattleActionStep _currentStep;

        public BattleByShieldState(RequiredRef requiredRef) : base(requiredRef)
        {
            _steps = new EnemyActionStep[1];
            _steps[0] = new HoldStep(requiredRef, null);

            _currentStep = _steps[0];
        }

        protected override void OnEnter()
        {
            Always();
        }

        protected override void OnExit()
        {
            Always();

            Vector3 p = Ref.Body.Position;
            AudioWrapper.PlaySE(p, "SE_PileBunker_Hit");
        }

        protected override void OnStay()
        {
            Always();

            if (ExitIfDeadOrTimeOver()) return;

            _currentStep = _currentStep.Update();
        }

        public override void Dispose()
        {
            foreach (EnemyActionStep s in _steps) s.Dispose();
        }

        private void Always()
        {
            PlayDamageSE();
            //Hovering();
            LeftRightMoveAnimation();
            ForwardBackMoveAnimation();
        }
    }    
}

namespace Enemy.Shield
{
    /// <summary>
    /// 盾を構える。
    /// </summary>
    public class HoldStep : EnemyActionStep
    {
        private Vector3 _velocity;

        public HoldStep(RequiredRef requiredRef, params EnemyActionStep[] next) : base(requiredRef, next)
        {
            // アイドルのアニメーション再生をトリガーする。
            {
                string state = Const.Shield.Idle;
                int layer = Const.Layer.BaseLayer;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnIdleAnimationStateEnter);
            }

            // 構えのアニメーション再生をトリガーする。
            {
                string state = Const.Shield.ShieldLoop;
                int layer = Const.Layer.BaseLayer;
                Ref.BodyAnimation.RegisterStateEnterCallback(ID, state, layer, OnHoldAnimationStateEnter);
            }
        }

        protected override void Enter()
        {
        }

        private void OnIdleAnimationStateEnter()
        {
            // 盾を構える。
            Ref.BodyAnimation.SetTrigger(Const.Param.AttackSet);
        }

        private void OnHoldAnimationStateEnter()
        {
            // 攻撃する処理は特にないので、盾構え後そのまま攻撃のアニメーション再生するだけ。
            Ref.BodyAnimation.SetTrigger(Const.Param.Attack);
        }

        protected override BattleActionStep Stay()
        {
            // 到達までのおおよその時間
            const float SmoothTime = 0.5f;

            Vector3 p = Ref.Body.Position;
            Vector3 bp = Ref.BlackBoard.Slot.Point;
            bp.z = p.z;
            Vector3 warp = Vector3.SmoothDamp(p, bp, ref _velocity, SmoothTime);
            Ref.Body.Warp(warp);

            return this;
        }

        public override void Dispose()
        {
            Ref.BodyAnimation.ReleaseStateCallback(ID);
        }
    }
}