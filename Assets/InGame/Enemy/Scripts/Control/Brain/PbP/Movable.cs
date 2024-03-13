namespace Enemy.Control.PbP
{
    /// <summary>
    /// 移動可能
    /// </summary>
    public class Movable : Node
    {
        private BlackBoard _blackBoard;

        public Movable(BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
        }

        public override bool PreCondition()
        {
            return 
                _blackBoard.IsAlive() && // 体力がある
                _blackBoard.IsFine() &&  // 大破していない
                _blackBoard.IsInTime();  // 生存時間以内
        }
    }
}