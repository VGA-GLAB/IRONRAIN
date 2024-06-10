using System.Collections.Generic;

namespace Enemy.Control.BT
{
    /// <summary>
    /// 子ノードを順に実行し、全ての子が成功した場合は成功を返す。
    /// </summary>
    public class Sequence : Node
    {
        private List<Node> _children;

        // 子が実行中の状態を返した場合に、次の呼び出しのタイミングで
        // 1番目の子から実行されるのを防ぐために、実行中の子の添え字を保持しておく。
        private int _current;

        public Sequence(string name = nameof(Sequence), params Node[] node) : base(name)
        {
            _children = new(node.Length);
            _children.AddRange(node);
        }

        protected override void OnBreak()
        {
            _current = 0;
            _children.ForEach(c => c.Break());
        }

        protected override void Enter()
        {
            _current = 0;
        }

        protected override void Exit()
        {
            _current = 0;
        }

        protected override State Stay()
        {
            while (_current < _children.Count)
            {
                State result = _children[_current].Update();

                // 子が成功した場合は次の呼び出しを待たずに次の子を実行
                if (result == State.Success) { _current++; continue; }

                // 子が実行中の場合はそれ以上実行しない。
                // 子が失敗した場合はこのノード自体も失敗を返すので、次の呼び出し時は最初の子から実行になる。
                return result;
            }

            // 全ての子が成功した場合は成功を返す。
            return State.Success;
        }

        /// <summary>
        /// 子ノードとして追加
        /// </summary>
        public void Add(Node node) => _children.Add(node);
    }
}
