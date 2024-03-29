namespace Enemy.Control
{
    /// <summary>
    /// BrainからActionへ、移動や攻撃など選択した行動を渡す。
    /// </summary>
    public struct ActionPlan
    {
        public Choice Choice;
    }
}
