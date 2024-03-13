namespace Enemy.Control.BT
{
    /// <summary>
    /// プレイヤーを向く。
    /// </summary>
    public class LookAtPlayer : Node
    {
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
            return State.Running; // テスト用
        }
    }
}
