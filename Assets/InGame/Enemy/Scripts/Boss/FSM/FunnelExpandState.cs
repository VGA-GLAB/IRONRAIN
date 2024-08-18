using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss.FSM
{
    /// <summary>
    /// 戦闘中、ファンネルを展開するステート。
    /// </summary>
    public class FunnelExpandState : BattleState
    {
        // アニメーションの再生と同時に展開後、一定時間待つ。
        private BattleActionStep _expand;
        private BattleActionStep _expandEnd;

        private BattleActionStep _currentStep;

        public FunnelExpandState(RequiredRef requiredRef) : base(requiredRef)
        {
            _expandEnd = new FunnelExpandEndStep();
            _expand = new FunnelExpandStep(requiredRef, _expandEnd);
        }

        protected override void Enter()
        {
            _blackBoard.CurrentState = StateKey.FunnelExpand;
            _currentStep = _expand;
        }

        protected override void Exit()
        {
            _expand.Reset();
            _expandEnd.Reset();
        }

        protected override void Stay()
        {
            PlayDamageSE();
            FunnelLaserSight();

            _currentStep = _currentStep.Update();

            // 終了ステップまで到達したらアイドル状態に戻る。
            if (_currentStep.ID == nameof(FunnelExpandEndStep)) TryChangeState(StateKey.Idle);
        }
    }

    /// <summary>
    /// その場でファンネルを展開。
    /// </summary>
    public class FunnelExpandStep : BattleActionStep
    {
        private BlackBoard _blackBoard;
        private Body _body;
        private BodyAnimation _animation;
        private IReadOnlyList<FunnelController> _funnels;
        private BattleActionStep _expandEnd;

        private float _timer;

        public FunnelExpandStep(RequiredRef requiredRef, BattleActionStep expandEnd)
        {
            _blackBoard = requiredRef.BlackBoard;
            _body = requiredRef.Body;
            _animation = requiredRef.BodyAnimation;
            _funnels = requiredRef.Funnels;
            _expandEnd = expandEnd;
        }

        public override string ID => nameof(FunnelExpandStep);

        protected override void Enter()
        {
            _timer = 2.0f; // アニメーションの再生時間に手動で合わせる。
            _animation.SetTrigger(BodyAnimationConst.Param.FunnelExpand);
            
            // ファンネル展開
            foreach (FunnelController f in _funnels) f.Expand();
            AudioWrapper.PlaySE("SE_Funnel");

            // このタイミングで黒板に実行を書き込んでいるが、ステートのExitでも良いかも？
            _blackBoard.FunnelExpand = Trigger.Executed;
        }

        protected override BattleActionStep Stay()
        {
            // 時間が来たら遷移。
            _timer -= _blackBoard.PausableDeltaTime;
            if (_timer > 0) return this;
            else return _expandEnd;
        }
    }

    /// <summary>
    /// ファンネル展開終了。
    /// </summary>
    public class FunnelExpandEndStep : BattleActionStep
    {
        public override string ID => nameof(FunnelExpandEndStep);

        protected override void Enter()
        {
        }

        protected override BattleActionStep Stay()
        {
            return this;
        }
    }
}
