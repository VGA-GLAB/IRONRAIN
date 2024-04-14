namespace Enemy.Control.BT
{
    /// <summary>
    /// プレイヤーを向く。
    /// </summary>
    public class LookAtPlayer : Node
    {
        private Choice _choice;
        private BlackBoard _blackBoard;

        public LookAtPlayer(Choice choice, BlackBoard blackBoard)
        {
            _choice = choice;
            _blackBoard = blackBoard;
        }

        protected override void OnBreak()
        {
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override State Stay()
        {
            // 前方向の値を変更することでプレイヤーの方向を向かせる。
            _blackBoard.AddForwardOption(_choice, _blackBoard.TransformToPlayerDirection);

            return State.Success;
        }
    }
}
