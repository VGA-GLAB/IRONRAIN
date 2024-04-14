namespace Enemy.Control
{
    /// <summary>
    /// 敵キャラクターを生成後、指定位置まで移動してくる処理を実装するインターフェース。
    /// </summary>
    public interface IApproach
    {
        /// <summary>
        /// 移動が完了したかどうかを返す。
        /// Updateのタイミングで呼ばれる。
        /// </summary>
        public bool IsCompleted();
    }
}
