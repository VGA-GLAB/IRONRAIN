﻿using UnityEngine;
using Enemy.Extensions;

namespace Enemy.Boss
{
    /// <summary>
    /// QTEの各ステップをIDで管理する。
    /// </summary>
    public abstract class QteEventStep
    {
        private bool _isEnter = true;

        public QteEventStep(RequiredRef requiredRef, QteEventStep next)
        {
            Ref = requiredRef;
            Next = next;
        }

        public abstract string ID { get; }
        protected RequiredRef Ref { get; private set; }
        protected QteEventStep Next { get; private set; }

        protected abstract void Enter();
        protected abstract QteEventStep Stay();

        /// <summary>
        /// 最初の1回はEnterが呼ばれ、以降はStayが呼ばれる。
        /// </summary>
        public QteEventStep Update()
        {
            if (_isEnter)
            {
                _isEnter = false;
                Enter();
                return this;
            }
            else
            {
                return Stay();
            }
        }

        /// <summary>
        /// ステートマシンを破棄するタイミングに合わせて諸々を破棄出来る。
        /// </summary>
        public virtual void Dispose() { }
    }

    /// <summary>
    /// 左腕破壊~2回目のQTEまでの一連のイベントのステート。
    /// </summary>
    public class QteEventState : State<StateKey>
    {
        private QteEventStep[] _steps;
        private QteEventStep _currentStep;

        public QteEventState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;

            _steps = new QteEventStep[8];
            _steps[7] = new CompleteStep(requiredRef, null);
            _steps[6] = new PenetrateStep(requiredRef, _steps[7]);
            _steps[5] = new knockBackStep(requiredRef, _steps[6]);
            _steps[4] = new SecondCombatStep(requiredRef, _steps[5]);
            _steps[3] = new knockBackStep(requiredRef, _steps[4]);
            _steps[2] = new FirstCombatStep(requiredRef, _steps[3]);
            _steps[1] = new BreakLeftArmStep(requiredRef, _steps[2]);
            _steps[0] = new MoveToPlayerFrontStep(requiredRef, _steps[1]);
        }

        private RequiredRef Ref { get; set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.QteEvent;

            _currentStep = _steps[0];
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            _currentStep = _currentStep.Update();
        }

        public override void Dispose()
        {
            foreach (QteEventStep s in _steps) s.Dispose();
        }
    }

    /// <summary>
    /// 刀を振り上げた状態でプレイヤーの正面に移動
    /// </summary>
    public class MoveToPlayerFrontStep : QteEventStep
    {
        public MoveToPlayerFrontStep(RequiredRef requiredRef, QteEventStep next) : base(requiredRef, next) { }

        public override string ID => nameof(MoveToPlayerFrontStep);

        protected override void Enter()
        {
            // 刀を構えるアニメーションはMoveからトリガーで遷移するよう設定されているが、
            // Attackの状態の時にQTEイベント開始を呼ばれる可能性があるので、ステートを指定で再生。
            string state = BodyAnimationConst.Boss.QteSwordSet;
            int layer = BodyAnimationConst.Layer.BaseLayer;
            Ref.BodyAnimation.Play(state, layer);
        }

        protected override QteEventStep Stay()
        {
            Vector3 v = VectorExtensions.Homing(
                Ref.BlackBoard.Area.Point,
                Ref.BlackBoard.PlayerArea.Point,
                Ref.BlackBoard.PlayerDirection,
                Ref.BossParams.Other.ApproachHomingPower
                );
            float spd = Ref.BossParams.Qte.ToPlayerFrontMoveSpeed;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            Vector3 velo = v * spd * dt;

            // 一定の距離以下になったらフラグを立てる。
            // 単純に直線距離で判定しているのでプレイヤーの後方から近づいても条件を満たしてしまう。
            float sqrDist = Ref.BlackBoard.PlayerSqrDistance;
            float qteSqrDist = Ref.BossParams.Qte.SocialSqrDistance;
            if (sqrDist < qteSqrDist)
            {
                Ref.BlackBoard.IsStandingOnQtePosition = true;
            }
            else
            {
                Vector3 area = Ref.BlackBoard.Area.Point;
                Ref.Body.Warp(area + velo);
            }

            // 回転
            Vector3 dir = Ref.BlackBoard.PlayerDirection;
            dir.y = 0;
            Ref.Body.LookForward(dir);

            // 左腕破壊の位置にいるフラグが立っているかつ、左腕破壊の命令がされている場合。
            bool isQtePosition = Ref.BlackBoard.IsStandingOnQtePosition;
            bool isOrdered = Ref.BlackBoard.IsBreakLeftArm;
            if (isQtePosition && isOrdered) return Next;
            else return this;
        }
    }

    /// <summary>
    /// 左腕破壊の演出、プレイヤーの入力で鍔迫り合いに遷移。
    /// </summary>
    public class BreakLeftArmStep : QteEventStep
    {
        public BreakLeftArmStep(RequiredRef requiredRef, QteEventStep next) : base(requiredRef, next) { }

        public override string ID => nameof(BreakLeftArmStep);

        protected override void Enter()
        {
            // プレイヤーの入力があった場合は、刀を振り下ろす。
            Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.QteBladeAttack01);
        }

        protected override QteEventStep Stay()
        {
            // 左腕破壊 -> 鍔迫り合い1回目 に遷移する命令がされた場合。
            bool isReady = Ref.BlackBoard.IsQteCombatReady;
            if (isReady) return Next;
            else return this;
        }
    }

    /// <summary>
    /// 鍔迫り合い1回目、刀を構え直してプレイヤーの入力で刀を振り下ろす。
    /// </summary>
    public class FirstCombatStep : QteEventStep
    {
        public FirstCombatStep(RequiredRef requiredRef, QteEventStep next) : base (requiredRef, next)
        {
            Ref.AnimationEvent.OnWeaponCrash += OnWeaponCrash;
        }

        public override string ID => nameof(FirstCombatStep);

        private void OnWeaponCrash()
        {
            Ref.Effector.PlayWeaponCrash();
        }

        protected override void Enter()
        {
            // 振り下ろした刀を構え直す。
            Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.QteBladeAttackClear01);
        }

        protected override QteEventStep Stay()
        {
            bool isInputed = Ref.BlackBoard.IsFirstCombatInputed;
            if (isInputed)
            {
                // 鍔迫り合い1回目、刀を振り下ろす。
                Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.QteBladeAttack02);
                
                return Next;
            }
            else
            {
                return this;
            }
        }

        public override void Dispose()
        {
            Ref.AnimationEvent.OnWeaponCrash -= OnWeaponCrash;
        }
    }

    /// <summary>
    /// 刀を振り下ろす -> 弾かれて吹き飛ばされる までがセット。
    /// ディレイを入れて弾かれるタイミングで移動させる。
    /// </summary>
    public class knockBackStep : QteEventStep
    {
        private Vector3 _force;
        private float _timer;

        public knockBackStep(RequiredRef requiredRef, QteEventStep next) : base(requiredRef, next) { }

        public override string ID => nameof(knockBackStep);

        protected override void Enter()
        {
            Vector3 back = -Ref.Body.Forward;
            float power = Ref.BossParams.Qte.KnockBackPower;
            _force = back * power;

            // 振り下ろすアニメーションの長さぶんだけディレイ。
            const float SwingDownDelay = 0.5f;
            _timer = SwingDownDelay;
        }

        protected override QteEventStep Stay()
        {
            // 刀を振り下ろしている最中は吹き飛ばない。
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _timer -= dt;
            if (_timer > 0) return this;
            else
            {
                Vector3 v = _force * dt;
                Ref.Body.Move(v);
            }

            const float Friction = 0.98f;
            _force *= Friction;

            // ある程度吹き飛ばされて、吹き飛ばし力が弱ったら鍔迫り合い2回目に遷移。
            const float StopThreshold = 1.0f;
            bool isOver = _force.sqrMagnitude < StopThreshold;
            if (isOver) return Next;
            else return this;
        }
    }

    /// <summary>
    /// 鍔迫り合い2回目、吹き飛ばされた状態から遷移してくる。
    /// 刀を構え直して突撃、プレイヤーの入力で振り下ろす。
    /// </summary>
    public class SecondCombatStep : QteEventStep
    {
        private Vector3 _force;

        public SecondCombatStep(RequiredRef requiredRef, QteEventStep next) : base(requiredRef, next)
        {
            Ref.AnimationEvent.OnWeaponCrash += OnWeaponCrash;
        }

        public override string ID => nameof(SecondCombatStep);

        private void OnWeaponCrash()
        {
            Ref.Effector.PlayWeaponCrash();
        }

        protected override void Enter()
        {
            // 振り下ろした刀を構え直す。
            // 1回目の鍔迫り合いのアニメーションを使いまわすのでトリガーではなくPlayで再生するしかない。
            string state = BodyAnimationConst.Boss.QteSowrdRepel_1;
            int layer = BodyAnimationConst.Layer.UpperBody;
            Ref.BodyAnimation.Play(state, layer);

            Vector3 dir = Ref.BlackBoard.PlayerDirection;
            float spd = Ref.BossParams.Qte.ChargeSpeed;
            _force = dir * spd;
        }

        protected override QteEventStep Stay()
        {
            // プレイヤーとの距離を詰める。
            float sqrDist = Ref.BlackBoard.PlayerSqrDistance;
            float sqrQteDist = Ref.BossParams.Qte.SocialSqrDistance;
            if (sqrDist > sqrQteDist)
            {
                float dt = Ref.BlackBoard.PausableDeltaTime;
                Vector3 v = _force * dt;
                Ref.Body.Move(v);

                // 距離を詰め切らないうちに突撃力が尽きないよう、最低値が必要。
                const float Friction = 0.98f;
                const float Min = 2.0f;
                if (_force.sqrMagnitude > Min) _force *= Friction;
            }

            // 鍔迫り合い2回目、刀を振り下ろす。
            // 現状、距離を詰め終わったかの判定をしていないのでシーケンス側で配慮が必要。
            bool isInputed = Ref.BlackBoard.IsSecondCombatInputed;
            if (isInputed)
            {
                Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.QteBladeAttack02);
                Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.QteBladeAttackClear02);
                
                return Next;
            }
            else
            {
                return this;
            }
        }

        public override void Dispose()
        {
            Ref.AnimationEvent.OnWeaponCrash -= OnWeaponCrash;
        }
    }

    /// <summary>
    /// パイルバンカーで貫かれる。
    /// </summary>
    public class PenetrateStep : QteEventStep
    {
        private Vector3 _force;

        public PenetrateStep(RequiredRef requiredRef, QteEventStep next) : base(requiredRef, next) { }

        public override string ID => nameof(PenetrateStep);

        protected override void Enter()
        {
            Vector3 dir = Ref.BlackBoard.PlayerDirection;
            float spd = Ref.BossParams.Qte.ChargeSpeed;
            _force = dir * spd;
        }

        protected override QteEventStep Stay()
        {
            // プレイヤーとの距離を詰める。
            float sqrDist = Ref.BlackBoard.PlayerSqrDistance;
            float sqrQteDist = Ref.BossParams.Qte.SocialSqrDistance;
            if (sqrDist > sqrQteDist)
            {
                float dt = Ref.BlackBoard.PausableDeltaTime;
                Vector3 v = _force * dt;
                Ref.Body.Move(v);

                // 距離を詰め切らないうちに突撃力が尽きないよう、最低値が必要。
                const float Friction = 0.98f;
                const float Min = 2.0f;
                if (_force.sqrMagnitude > Min) _force *= Friction;
            }

            // QTE2回目、プレイヤーに殴られて死亡。
            // 現状、距離を詰め終わったかの判定をしていないのでシーケンス側で配慮が必要。
            bool isInputed = Ref.BlackBoard.IsPenetrateInputed;
            if (isInputed)
            {
                Ref.BodyAnimation.SetTrigger(BodyAnimationConst.Param.Finish);
                Ref.Effector.PlayDestroyed();

                AudioWrapper.PlaySE("SE_Kill");

                return Next;
            }
            else
            {
                return this;
            }
        }
    }

    /// <summary>
    /// QTEイベントの全操作が完了。
    /// </summary>
    public class CompleteStep : QteEventStep
    {
        public CompleteStep(RequiredRef requiredRef, QteEventStep next) : base(requiredRef, next) { }

        public override string ID => nameof(CompleteStep);

        protected override void Enter()
        {
            AgentScript agent = Ref.AgentScript;
            if (agent != null) agent.EnemyDestory();
        }

        protected override QteEventStep Stay()
        {
            return this;
        }
    }
}