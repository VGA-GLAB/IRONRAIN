namespace Enemy.Control
{
    /// <summary>
    /// 装備ごとの攻撃判定やアニメーションの差を吸収するインターフェース。
    /// </summary>
    public interface IEquipment
    {
        /// <summary>
        /// 装備者側の状態やタイミングに合わせるために必要な諸々の参照を渡す。
        /// </summary>
        void RegisterOwner(IOwnerTime ownerTime);
    }
}