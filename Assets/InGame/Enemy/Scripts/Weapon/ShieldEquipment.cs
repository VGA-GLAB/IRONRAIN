namespace Enemy
{
    public class ShieldEquipment : MeleeEquipment
    {
        protected override void OnCollision()
        {
            AudioWrapper.PlaySE("SE_Shield");
        }
    }
}
