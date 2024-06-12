namespace Enemy.Control.BT
{
    /// <summary>
    /// ビヘイビアツリーの各種ノードはこのクラスを継承する。
    /// </summary>
    public abstract class Node
    {
        public enum State
        {
            Running,
            Failure,
            Success,
        }

        private State _state;
        private bool _isActive;

        public Node(string name = null)
        {
            Name = name ?? "Node";
        }

        /// <summary>
        /// ログやUI等に表示するためのノード名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 1回以上Updateを呼んだ状態からノードを強制的に初期状態に戻す際に呼ぶ。
        /// </summary>
        public void Break()
        {
            _state = State.Running;
            _isActive = false;

            OnBreak();
        }

        /// <summary>
        /// 1度の呼び出しで最初の1回はEnterとStayが呼ばれる。
        /// StayがRunning以外を返した場合はStayとExitが呼ばれる
        /// </summary>
        public State Update()
        {
            if (!_isActive)
            {
                _isActive = true;
                Enter();
            }

#if UNITY_EDITOR
            //Debug.Log(NodeName + "を実行中");
#endif

            _state = Stay();

            if (_state == State.Failure || _state == State.Success)
            {
                Exit();
                _isActive = false;
            }

            return _state;
        }

        protected virtual void OnBreak() { }
        protected abstract void Enter();
        protected abstract State Stay();
        protected abstract void Exit();
    }
}