namespace Enemy.Control
{
    /// <summary>
    /// 武器として扱うインターフェース
    /// </summary>
    public interface IWeapon
    {
        /// <summary>
        /// 攻撃する。
        /// </summary>
        public void Attack();
    }
}