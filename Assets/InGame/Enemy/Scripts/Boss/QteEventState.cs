using Enemy.Funnel;
using UnityEngine;

namespace Enemy.Boss
{
    /// <summary>
    /// 左腕破壊~2回目のQTEまでの一連のイベントのステート。
    /// </summary>
    public class QteEventState : State<StateKey>
    {
        private BossActionStep[] _qteSteps;
        private BattleActionStep _currentQteStep;
        // プレイヤーの正面のレーンに移動と並列して向かしておく。
        private BossActionStep[] _lookSteps;
        private BattleActionStep _currentLookStep;

        public QteEventState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;

            _qteSteps = new BossActionStep[11];
            _qteSteps[10] = new CompleteStep(requiredRef, null);
            _qteSteps[9] = new PenetrateStep(requiredRef, _qteSteps[10]);
            _qteSteps[8] = new FinalChargeStep(requiredRef, _qteSteps[9]);
            _qteSteps[7] = new SecondKnockBackStep(requiredRef, _qteSteps[8]);
            _qteSteps[6] = new SecondCombatStep(requiredRef, _qteSteps[7]);
            _qteSteps[5] = new SecondChargeStep(requiredRef, _qteSteps[6]);
            _qteSteps[4] = new FirstKnockBackStep(requiredRef, _qteSteps[5]);
            _qteSteps[3] = new FirstCombatStep(requiredRef, _qteSteps[4]);
            _qteSteps[2] = new BreakLeftArmStep(requiredRef, _qteSteps[3]);
            _qteSteps[1] = new FirstChargeStep(requiredRef, _qteSteps[2]);
            _qteSteps[0] = new LaneChangeToPlayerFrontStep(requiredRef, _qteSteps[1]);

            _lookSteps = new BossActionStep[2];
            _lookSteps[1] = new CompleteStep(requiredRef, null);
            _lookSteps[0] = new ParallelLookAtPlayerStep(requiredRef, _lookSteps[1]);
        }

        private RequiredRef Ref { get; set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.QteEvent;

            _currentQteStep = _qteSteps[0];
            _currentLookStep = _lookSteps[0];

            // QTEが始まったらファンネルに攻撃をやめさせる。
            foreach (FunnelController f in Ref.Funnels) f.FireEnable(false);
        }

        protected override void Exit()
        {
            Debug.Log("一連のQTEイベントが終了");
        }

        protected override void Stay()
        {
            _currentQteStep = _currentQteStep.Update();
            _currentLookStep = _currentLookStep.Update();

            bool isQteEnd = _currentQteStep.ID == nameof(CompleteStep);
            bool isLookEnd = _currentLookStep.ID == nameof(CompleteStep);
            if (isQteEnd && isLookEnd) TryChangeState(StateKey.Defeated);
        }

        public override void Dispose()
        {
            foreach (BossActionStep s in _qteSteps) s.Dispose();
        }
    }

    /// <summary>
    /// プレイヤーの正面レーンに移動。
    /// </summary>
    public class LaneChangeToPlayerFrontStep : BossActionStep
    {
        // Lerpで移動。
        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;
        // このステップ内で複数回移動させる。
        private int _rest;
        private int _sign;
        // 移動開始のタイミングで代入、移動完了後にこの値を黒板に書き込む。
        private int _nextIndex;

        public LaneChangeToPlayerFrontStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            // 現在のレーンから時計回りと反時計回りで移動する場合の移動回数が少ない方を選択。
            int clockwise = Ref.Field.GetClockwiseMoveCount();
            int counterclockwise = Ref.Field.GetCounterClockwiseMoveCount();
            if (clockwise <= counterclockwise)
            {
                // 時計回り
                _rest = clockwise;
                _sign = -1;
            }
            else
            {
                // 反時計回り
                _rest = counterclockwise;
                _sign = 1;
            }

            // 既にプレイヤーの反対レーンにいる場合は移動しない。
            if (_rest > 0) NextLane();
            else
            {
                _start = Ref.Body.Position;
                _end = _start;
                _lerp = 0;
                _nextIndex = Ref.BlackBoard.CurrentLaneIndex;
            }
        }

        protected override BattleActionStep Stay()
        {
            if (_lerp >= 1)
            {
                Ref.BlackBoard.CurrentLaneIndex = _nextIndex;

                if (_rest == 0) return Next[0];
                else NextLane();
            }

            Vector3 p = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.Warp(p);

            // レーン間の移動速度。
            const float Speed = 6.0f;

            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * Speed;
            _lerp = Mathf.Clamp01(_lerp);

            return this;
        }

        // 移動先のレーンを更新。
        private void NextLane()
        {
            _rest--;

            if (_sign == -1) _nextIndex = Ref.Field.GetLeftLaneIndex();
            else if (_sign == 1) _nextIndex = Ref.Field.GetRightLaneIndex();

            _start = Ref.Body.Position;
            _end = Ref.Field.GetLanePointWithOffset(_nextIndex);
            _lerp = 0;
        }
    }

    /// <summary>
    /// プレイヤーに向けて回転。レーン移動と並行して行う。
    /// </summary>
    public class ParallelLookAtPlayerStep : BossActionStep
    {
        private Vector3 _start;
        private Vector3 _end;
        private float _lerp;
        private int _diff;

        public ParallelLookAtPlayerStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            _start = Ref.Body.Forward;
            // ボス戦のフィールドの中心からプレイヤーのレーンへのベクトル。
            // レーン移動終了時にはこの正面のレーンに移動する予定なので、この方向を向く。
            Vector3 pl = Ref.Field.GetPlayerLane();
            pl.y = 0;
            _end = pl;
            _lerp = 0;

            _diff = Ref.Field.GetMinMoveCount();
        }

        protected override BattleActionStep Stay()
        {
            Vector3 look = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.LookForward(look);

            // 振り向き速度。
            const float Speed = 100.0f;

            float dt = Ref.BlackBoard.PausableDeltaTime;
            _lerp += dt * (Speed / _diff);

            if (_lerp > 1.0f) return Next[0];
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

        public FirstChargeStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            _start = Ref.Body.Position;
            Vector3 dir = Ref.Field.GetCurrentLane().normalized;
            float dist = Ref.BossParams.BreakLeftArm.Distance;
            _end = Ref.Field.PointP() + dir * dist;

            // 刀を構えるアニメーションはMoveからトリガーで遷移するよう設定されているが、
            // それ以外の状態の時にQTEイベント開始を呼ばれる可能性があるので、ステートを指定で再生。
            string state = Const.Boss.QteSwordSet;
            int layer = Const.Layer.BaseLayer;
            Ref.BodyAnimation.Play(state, layer);
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
            if (isQtePosition && isOrderd) return Next[0];

            Vector3 before = Ref.Body.Position;
            Vector3 p = Vector3.Lerp(_start, _end, _lerp);
            Ref.Body.Warp(p);
            Vector3 after = Ref.Body.Position;
            Vector3 look = before - after;
            //if (look != Vector3.zero) Ref.Body.LookForward(before - after);

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
        public BreakLeftArmStep(RequiredRef requiredRef, BossActionStep next) : base(requiredRef, next) { }

        protected override void Enter()
        {
            // プレイヤーの入力があった場合は、刀を振り下ろす。
            Ref.BodyAnimation.SetTrigger(Const.Param.QteBladeAttack01);
        }

        protected override BattleActionStep Stay()
        {
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

        public FirstCombatStep(RequiredRef requiredRef, BossActionStep next) : base (requiredRef, next)
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
            Ref.Body.Move(v);

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
            _end = Ref.Field.PointP() + dir * dist;

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
            Ref.Body.Move(v);

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
            float dist = Ref.BossParams.SecondQte.Distance;
            _end = Ref.Field.PointP() + dir * dist;

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

            float speed = Ref.BossParams.SecondQte.MoveSpeed;
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