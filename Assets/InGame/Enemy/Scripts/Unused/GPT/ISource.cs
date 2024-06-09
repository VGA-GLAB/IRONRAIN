namespace Enemy.Unused.GPT
{
    /// <summary>
    /// AIへリクエストする用のパラメータを扱う。
    /// 受信したメッセージを基に計算し、リクエストのタイミングで値を取得する。
    /// </summary>
    public interface ISource<T1, T2>
    {
        public T1 Source { get; }
        public void Calculate(T2 msg);
    }
}
