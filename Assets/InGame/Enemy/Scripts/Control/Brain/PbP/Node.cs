namespace Enemy.Control.PbP
{
    /// <summary>
    /// 評価木の各種ノードはこのクラスを継承する。
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// 前提条件
        /// </summary>
        public abstract bool PreCondition();
    }
}
