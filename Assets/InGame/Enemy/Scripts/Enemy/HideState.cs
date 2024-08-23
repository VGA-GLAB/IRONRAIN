namespace Enemy
{
    /// <summary>
    /// 生成後、画面に表示されていない状態のステート。
    /// </summary>
    public class HideState : State<StateKey>
    {
        public HideState(RequiredRef requiredRef) : base(requiredRef.States)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        protected override void Enter()
        {
            Ref.BlackBoard.CurrentState = StateKey.Hide;

            Ref.Body.RendererEnable(false);         
        }

        protected override void Exit()
        {
            // 接近する場合は生存中のフラグが立っているので画面に表示させる。
            bool isAlive = Ref.BlackBoard.IsAlive;
            Ref.Body.RendererEnable(isAlive);
        }

        protected override void Stay()
        {
            // プレイヤーを検知した場合は接近
            bool isDetect = Ref.BlackBoard.IsPlayerDetect;
            if (isDetect) { TryChangeState(StateKey.Approach); return; }

            // 死亡した場合は削除
            bool isDead = !Ref.BlackBoard.IsAlive;
            if (isDead) { TryChangeState(StateKey.Delete); return; }
        }
    }
}