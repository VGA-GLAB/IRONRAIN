namespace Enemy.Control
{
    /// <summary>
    /// 装備ごとの攻撃判定やアニメーションの差を吸収するインターフェース。
    /// ステートマシン内のステートから呼び出される。
    /// </summary>
    public interface IEquipment
    {
        /// <summary>
        /// 攻撃判定
        /// </summary>
        public void Attack(IOwnerTime ownerTime);
        /// <summary>
        /// 攻撃のアニメーションを再生
        /// Brain側が攻撃を選択したフレームの間呼び出され続ける。
        /// </summary>
        public void PlayAttackAnimation(BodyAnimation animation);
        /// <summary>
        /// 攻撃終了時のアニメーションを再生
        /// Animator側で攻撃のアニメーションの再生が終了したタイミングで呼び出される。
        /// </summary>
        public void PlayAttackEndAnimation(BodyAnimation animation);
    }
}