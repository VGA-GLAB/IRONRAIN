namespace Enemy.Control
{
    /// <summary>
    /// MonoBehaviourの各イベント関数を実装する。
    /// </summary>
    public interface ILifeCycleHandler
    {
        public void OnStartEvent() { }
        public void OnEnableEvent() { }
        public void OnDisableEvent() { }
        public void UpdateEvent() { }
        public void LateUpdateEvent() { }
        public void OnDrawGizmosEvent() { }
    }
}
