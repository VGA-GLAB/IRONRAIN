namespace Enemy
{
    /// <summary>
    /// 戦闘中の各行動を、更に動作単位で分けて管理する。
    /// </summary>
    public abstract class BattleActionStep
    {
        private bool _isEnter = true;

        public BattleActionStep(params BattleActionStep[] next)
        {
            Next = next;
        }

        public string ID => GetType().Name;
        protected BattleActionStep[] Next {  get; private set; }

        protected abstract void Enter();
        protected abstract BattleActionStep Stay();

        /// <summary>
        /// 最初の1回はEnterが呼ばれ、以降はStayが呼ばれる。
        /// </summary>
        public BattleActionStep Update()
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
        /// 再度Enterから呼ばれるようになる。
        /// </summary>
        public void Reset() => _isEnter = true;

        /// <summary>
        /// ステートマシンを破棄するタイミングに合わせて諸々を破棄出来る。
        /// </summary>
        public virtual void Dispose() { }
    }
}
