using Enemy.Funnel;
using UnityEngine;
using Enemy.Boss.Qte;
using Unity.VisualScripting;

namespace Enemy.Boss
{
    /// <summary>
    /// 左腕破壊~2回目のQTEまでの一連のイベントのステート。
    /// </summary>
    public class QteEventState : BattleState
    {
        private BossActionStep[] _steps;
        private BattleActionStep _currentStep;

        public QteEventState(RequiredRef requiredRef) : base(requiredRef)
        {
            _steps = new BossActionStep[10];
            _steps[9] = new CompleteStep(requiredRef, null);
            _steps[8] = new PenetrateStep(requiredRef, _steps[9]);
            _steps[7] = new FinalChargeStep(requiredRef, _steps[8]);
            //_steps[9] = new SecondKnockBackStep(requiredRef, _steps[10]);
            //_steps[8] = new SecondCombatStep(requiredRef, _steps[9]);
            //_steps[7] = new SecondChargeStep(requiredRef, _steps[8]);
            _steps[6] = new FirstKnockBackStep(requiredRef, _steps[7]);
            _steps[5] = new FirstCombatStep(requiredRef, _steps[6]);
            _steps[4] = new BreakLeftArmStep(requiredRef, _steps[5]);
            _steps[3] = new FirstChargeStep(requiredRef, _steps[4]);
            _steps[2] = new BladeOpenStep(requiredRef, _steps[3]);
            _steps[1] = new WaitAnimationStep(requiredRef, _steps[2]);
            _steps[0] = new LaneChangeStep(requiredRef, _steps[1]);
        }

        protected override void OnEnter()
        {
            Ref.BlackBoard.CurrentState = StateKey.QteEvent;

            _currentStep = _steps[0];

            TurnToPlayer(isReset: true);

            // QTEが始まったらファンネルに攻撃をやめさせる。
            foreach (FunnelController f in Ref.Funnels) f.FireEnable(false);
        }

        protected override void OnExit()
        {
        }

        protected override void OnStay()
        {
            TurnToPlayer();

            _currentStep = _currentStep.Update();

            if (_currentStep.ID == nameof(CompleteStep)) TryChangeState(StateKey.Defeated);
        }

        public override void Dispose()
        {
            foreach (BossActionStep s in _steps) s.Dispose();
        }
    }
}

namespace Enemy.Boss.Qte
{
    /// <summary>
    /// プレイヤーの正面レーンに移動。
    /// </summary>
    public class LaneChangeStep : LaneChange.LaneChangeStep
    {
        public LaneChangeStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) 
        {
            // レーンチェンジのものと全く同じ。
        }
    }

    /// <summary>
    /// 左右移動のアニメーションを待つ。
    /// </summary>
    public class WaitAnimationStep : LaneChange.WaitAnimationStep
    {
        public WaitAnimationStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next)
        {
            // レーンチェンジのものと全く同じ。
        }
    }

    /// <summary>
    /// 刀を展開。
    /// </summary>
    public class BladeOpenStep : BossActionStep
    {
        private float _elapsed;
        private bool _isOpen;

        public BladeOpenStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next)
        {
            // アニメーションに合わせて刀を展開。
            Ref.AnimationEvent.OnQteBladeOpen += OnQteBladeOpen;
        }

        private void OnQteBladeOpen()
        {
            BladeEquipment blade = Ref.MeleeEquip as BladeEquipment;
            blade.Open();

            _isOpen = true;
        }

        protected override void Enter()
        {
            // 刀を構えるアニメーションはMoveからトリガーで遷移するよう設定されているが、
            // それ以外の状態の時にQTEイベント開始を呼ばれる可能性があるので、ステートを指定で再生。
            string state = Const.Boss.QteSwordSet;
            int layer = Const.Layer.BaseLayer;
            Ref.BodyAnimation.Play(state, layer);

            _elapsed = 0;
            _isOpen = false;
        }

        protected override BattleActionStep Stay()
        {
            // アニメーション終了を正確に待つため、刀展開後にカウント開始。
            if (_isOpen)
            {
                float dt = Ref.BlackBoard.PausableDeltaTime;
                _elapsed += dt;
            }

            // 良い感じに見せるため、刀が展開されたタイミングからディレイを付ける。
            const float Delay = 1.0f;

            if (_elapsed > Delay) return Next[0];
            else return this;
        }
    }

    /// <summary>
    /// 刀を振り上げた状態でプレイヤーの正面に突撃。
    /// </summary>
    public class FirstChargeStep : BossActionStep
    {
        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;

        public FirstChargeStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) 
        {
        }

        protected override void Enter()
        {
            _start = Ref.Body.Position;
            Vector3 dir = Ref.Field.GetCurrentLane().normalized;
            float dist = Ref.BossParams.BreakLeftArm.Distance;
            Vector3 offset = Ref.BossParams.BreakLeftArm.Offset;
            _end = Ref.Player.position + dir * dist + offset;
        }

        protected override BattleActionStep Stay()
        {
            if (_lerp >= 1)
            {
                // シーケンス側に位置に着いたことを知らせるためのフラグを立てる。
                Ref.BlackBoard.IsStandingOnQtePosition = true;
            }

            // 位置に着いた後、シーケンス側からの呼び出しで次のステップに遷移。
            bool isQtePosition = Ref.BlackBoard.IsStandingOnQtePosition;
            bool isOrderd = Ref.BlackBoard.IsBreakLeftArm;
            if (isQtePosition && isOrderd)
            {
                // 見てくれの邪魔なので切断と同時にロケットランチャーを消す。
                Ref.RangeEquip.RendererDisable();
                return Next[0];
            }

            Vector3 p = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.Warp(p);

            float speed = Ref.BossParams.BreakLeftArm.MoveSpeed;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * speed;
            _lerp = Mathf.Clamp01(_lerp);

            return this;
        }
    }

    /// <summary>
    /// 左腕破壊の演出、プレイヤーの入力で鍔迫り合いに遷移。
    /// </summary>
    public class BreakLeftArmStep : BossActionStep
    {
        // アニメーションの再生時間を手動で指定。
        private const float AnimationEnd = 2.15f;

        // アニメーションイベントの再生を待つため、一定時間の経過が必要。
        private float _elapsed;

        public BreakLeftArmStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            // プレイヤーの入力があった場合は、刀を振り下ろす。
            Ref.BodyAnimation.SetTrigger(Const.Param.QteBladeAttack01);

            _elapsed = 0;
        }

        protected override BattleActionStep Stay()
        {
            // アニメーションイベントの再生が終わるまでは遷移させない。
            if (_elapsed < AnimationEnd)
            {
                float dt = Ref.BlackBoard.PausableDeltaTime;
                _elapsed += dt;
                return this;
            }

            // 左腕破壊 -> 鍔迫り合い1回目 に遷移する命令がされた場合。
            bool isReady = Ref.BlackBoard.IsQteCombatReady;
            if (isReady) return Next[0];
            else return this;
        }
    }

    /// <summary>
    /// 鍔迫り合い1回目、刀を構え直してプレイヤーの入力で刀を振り下ろす。
    /// </summary>
    public class FirstCombatStep : BossActionStep
    {
        // このステップを実行中かのフラグ。
        private bool _isRunning;
        // 次のステップに遷移指せるかのフラグ。
        private bool _isTransition;

        public FirstCombatStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next)
        {
            Ref.AnimationEvent.OnWeaponCrash += OnWeaponCrash;
        }

        private void OnWeaponCrash()
        {
            // このステップを実行していない時は弾く。
            if (!_isRunning) return;

            Ref.Effector.PlayWeaponCrash();

            // 武器同士がぶつかったタイミングで次のステップに遷移させる。
            _isTransition = true;
        }

        protected override void Enter()
        {
            // 振り下ろした刀を構え直す。
            Ref.BodyAnimation.SetTrigger(Const.Param.QteBladeAttackClear01);

            _isRunning = true;
        }

        protected override BattleActionStep Stay()
        {
            // プレイヤー側の入力があった場合、鍔迫り合い1回目、刀を振り下ろす。
            bool isInputed = Ref.BlackBoard.IsFirstCombatInputed;
            if (isInputed) Ref.BodyAnimation.SetTrigger(Const.Param.QteBladeAttack02);

            if (_isTransition)
            {
                _isRunning = false;
                _isTransition = false;
                return Next[0];
            }
            else return this;
        }

        public override void Dispose()
        {
            Ref.AnimationEvent.OnWeaponCrash -= OnWeaponCrash;
        }
    }

    /// <summary>
    /// 1回目の鍔迫り合い、刀が弾かれたタイミングでノックバックする。
    /// </summary>
    public class FirstKnockBackStep : BossActionStep
    {
        private Vector3 _velocity;

        public FirstKnockBackStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            Vector3 back = -Ref.Body.Forward;
            float power = Ref.BossParams.FirstQte.KnockBack;
            _velocity = back * power;
        }

        protected override BattleActionStep Stay()
        {
            // 刀を振り下ろしている最中は吹き飛ばない。
            float dt = Ref.BlackBoard.PausableDeltaTime;
            Vector3 v = _velocity * dt;
            //Ref.Body.Move(v); <-ノックバックが仕様でないなったので一旦コメントアウト。

            // 摩擦力で止める。
            const float Friction = 0.98f;
            _velocity *= Friction;

            // ある程度吹き飛ばされて、速度がほぼ0になったら鍔迫り合い2回目に遷移。
            const float StopThreshold = 1.0f;
            if (_velocity.sqrMagnitude < StopThreshold) return Next[0];
            else return this;
        }
    }

    /// <summary>
    /// ノックバックされた状態から再度プレイヤーの正面に突撃。
    /// </summary>
    public class SecondChargeStep : BossActionStep
    {
        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;

        public SecondChargeStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            _start = Ref.Body.Position;
            Vector3 dir = Ref.Field.GetCurrentLane().normalized;
            float dist = Ref.BossParams.SecondQte.Distance;
            Vector3 offset = Ref.BossParams.SecondQte.Offset;
            _end = Ref.Player.position + dir * dist + offset;

            // 2回目の再生はトリガーで遷移出来ないので、ステートを指定して再生。
            string state = Const.Boss.QteSwordHold_2;
            int layer = Const.Layer.BaseLayer;
            Ref.BodyAnimation.Play(state, layer);

            // トリガーをリセットしないと 構え->攻撃 が再生されてしまう。
            string trigger = Const.Param.QteBladeAttack02;
            Ref.BodyAnimation.ResetTrigger(trigger);
        }

        protected override BattleActionStep Stay()
        {
            Vector3 p = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.Warp(p);

            // 移動完了した後、プレイヤーからの入力があったら次のステップに遷移。
            bool isInputed = Ref.BlackBoard.IsSecondCombatInputed;
            if (_lerp >= 1.0f && isInputed) return Next[0];

            float speed = Ref.BossParams.SecondQte.MoveSpeed;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * speed;
            _lerp = Mathf.Clamp01(_lerp);

            return this;
        }
    }

    /// <summary>
    /// 鍔迫り合い2回目、刀を振り下ろす。
    /// </summary>
    public class SecondCombatStep : BossActionStep
    {
        // このステップを実行中かのフラグ。
        private bool _isRunning;
        // 次のステップに遷移指せるかのフラグ。
        private bool _isTransition;

        public SecondCombatStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next)
        {
            Ref.AnimationEvent.OnWeaponCrash += OnWeaponCrash;
        }

        private void OnWeaponCrash()
        {
            // このステップを実行していない時は弾く。
            if (!_isRunning) return;

            Ref.Effector.PlayWeaponCrash();

            // 武器同士がぶつかったタイミングで次のステップに遷移させる。
            _isTransition = true;
        }

        protected override void Enter()
        {
            // 刀を振り下ろす。
            Ref.BodyAnimation.SetTrigger(Const.Param.QteBladeAttack02);

            _isRunning = true;
        }

        protected override BattleActionStep Stay()
        {
            if (_isTransition)
            {
                _isRunning = false;
                _isTransition = false;
                return Next[0];
            }
            else return this;
        }

        public override void Dispose()
        {
            Ref.AnimationEvent.OnWeaponCrash -= OnWeaponCrash;
        }
    }

    /// <summary>
    /// 2回目の鍔迫り合い、刀が弾かれたタイミングでノックバックする。
    /// </summary>
    public class SecondKnockBackStep : BossActionStep
    {
        private Vector3 _velocity;

        public SecondKnockBackStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            Vector3 back = -Ref.Body.Forward;
            float power = Ref.BossParams.SecondQte.KnockBack;
            _velocity = back * power;
        }

        protected override BattleActionStep Stay()
        {
            // 刀を振り下ろしている最中は吹き飛ばない。
            float dt = Ref.BlackBoard.PausableDeltaTime;
            Vector3 v = _velocity * dt;
            // Ref.Body.Move(v); <-ノックバックが仕様でないなったので一旦コメントアウト。

            // 摩擦力で止める。
            const float Friction = 0.98f;
            _velocity *= Friction;

            // ある程度吹き飛ばされて、速度がほぼ0になったら鍔迫り合い2回目に遷移。
            const float StopThreshold = 1.0f;
            if (_velocity.sqrMagnitude < StopThreshold) return Next[0];
            else return this;
        }
    }

    /// <summary>
    /// ノックバックされた状態から再々度プレイヤーの正面に突撃。
    /// </summary>
    public class FinalChargeStep : BossActionStep
    {
        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;

        public FinalChargeStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            _start = Ref.Body.Position;
            Vector3 dir = Ref.Field.GetCurrentLane().normalized;
            float dist = Ref.BossParams.FinalQte.Distance;
            Vector3 offset = Ref.BossParams.FinalQte.Offset;
            _end = Ref.Player.position + dir * dist + offset;

            // 3回目の再生はトリガーで遷移出来ないので、ステートを指定して再生。
            string state = Const.Boss.QteSwordHold_2;
            int layer = Const.Layer.BaseLayer;
            Ref.BodyAnimation.Play(state, layer);

            // トリガーをリセットしないと 構え->攻撃 が再生されてしまう。
            string trigger = Const.Param.QteBladeAttack02;
            Ref.BodyAnimation.ResetTrigger(trigger);
        }

        protected override BattleActionStep Stay()
        {
            Vector3 p = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.Warp(p);

            // 移動完了した後、プレイヤーからの入力があったら次のステップに遷移。
            bool isInputed = Ref.BlackBoard.IsPenetrateInputed;
            if (_lerp >= 1.0f && isInputed) return Next[0];

            float speed = Ref.BossParams.FinalQte.MoveSpeed;
            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * speed;
            _lerp = Mathf.Clamp01(_lerp);

            return this;
        }
    }

    /// <summary>
    /// パイルバンカーで貫かれる。
    /// </summary>
    public class PenetrateStep : BossActionStep
    {
        public PenetrateStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            // 遷移先に指定されていないのでステートを指定して再生する。
            string state = Const.Boss.BossFinish;
            int layer = Const.Layer.BaseLayer;
            Ref.BodyAnimation.Play(state, layer);
            Ref.Effector.PlayDestroyed();

            int thruster = Ref.BlackBoard.ThrusterSE;
            int jet = Ref.BlackBoard.JetSE;
            AudioWrapper.StopSE(thruster);
            AudioWrapper.StopSE(jet);

            Vector3 p = Ref.Body.Position;
            AudioWrapper.PlaySE(p, "SE_Kill");
        }

        protected override BattleActionStep Stay()
        {
            return this;
        }
    }

    /// <summary>
    /// QTEイベントの全操作が完了。
    /// </summary>
    public class CompleteStep : BossActionStep
    {
        public CompleteStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            return this;
        }
    }
}