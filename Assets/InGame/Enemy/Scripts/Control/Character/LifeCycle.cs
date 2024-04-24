namespace Enemy.Control
{
    /// <summary>
    /// EnemyControllerクラスが呼び出すメソッド群
    /// </summary>
    public abstract class LifeCycle
    {
        /// <summary>
        /// 各階層の処理の実行結果を返す。
        /// </summary>
        public enum Result
        {
            Running,  // 基本値、実行中の場合
            Complete, // これ以上処理を実行しない場合
        }

        // MonoBehaviourの各イベント関数
        public virtual void OnStartEvent() { }
        public virtual void OnEnableEvent() { }
        public virtual void OnDisableEvent() { }
        public virtual void OnDestroyEvent() { }
        public virtual Result UpdateEvent() { return Result.Running; }
        public virtual Result LateUpdateEvent() { return Result.Running; }
        public virtual void OnDrawGizmosEvent() { }

        // インターフェースにフックする処理
        public virtual void OnDamaged(int value, string weapon) { }

        // このメソッドを呼んだ次のフレームで非表示になる
        public virtual void OnPreCleanup() { }
    }
}
