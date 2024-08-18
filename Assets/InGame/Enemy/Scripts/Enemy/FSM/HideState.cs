namespace Enemy.FSM
{
    /// <summary>
    /// 生成後、画面に表示されていない状態のステート。
    /// </summary>
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
            // 接近する場合は生存中のフラグが立っているので画面に表示させる。
            _body.RendererEnable(_blackBoard.IsAlive);
        }

        protected override void Stay()
        {
            // プレイヤーを検知した場合は接近
            if (_blackBoard.IsPlayerDetect) TryChangeState(StateKey.Approach);
            // 死亡した場合は削除
            else if (!_blackBoard.IsAlive) TryChangeState(StateKey.Delete);
        }
    }
}