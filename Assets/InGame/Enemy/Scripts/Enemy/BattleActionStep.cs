using System.Collections.Generic;
using System.Linq;

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
            // 後から追加する前提だとnullになる。
            if (next == null) Next = new List<BattleActionStep>();
            else Next = next.ToList();
        }

        public string ID => GetType().Name;
        protected List<BattleActionStep> Next { get; }

        protected abstract void Enter();
        protected abstract BattleActionStep Stay();

        /// <summary>
        /// 遷移先を追加する。
        /// 基本はコンストラクタだが、一連の流れを繰り返す場合は、後から追加する必要がある。
        /// </summary>
        public void AddNext(BattleActionStep next)
        {
            Next.Add(next);
        }

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
                BattleActionStep next = Stay();
                // 違うステップに遷移した場合はリセット。
                if (ID != next.ID) _isEnter = true;

                return next;
            }
        }

        /// <summary>
        /// ステートマシンを破棄するタイミングに合わせて諸々を破棄出来る。
        /// </summary>
        public virtual void Dispose() { }
    }
}
