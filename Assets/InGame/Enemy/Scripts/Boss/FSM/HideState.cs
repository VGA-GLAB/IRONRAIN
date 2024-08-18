﻿namespace Enemy.Boss.FSM
{
    public class HideState : State
    {
        private BlackBoard _blackBoard;
        private Body _body;

        public HideState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            _blackBoard = requiredRef.BlackBoard;
            _body = requiredRef.Body;
        }

        protected override void Enter()
        {
            _blackBoard.CurrentState = StateKey.Hide;

            _body.RendererEnable(false);
        }

        protected override void Exit()
        {
            _body.RendererEnable(true);
        }

        protected override void Stay()
        {
            // ボス戦が始まった場合は登場状態に遷移。
            if (_blackBoard.IsBossStarted)
            {
                TryChangeState(StateKey.Appear);
            }
        }
    }
}
