namespace Enemy
{
    public class BladeEquipment : MeleeEquipment
    {
        protected override void OnCollision()
        {
            AudioWrapper.PlaySE("SE_Sword");
        }
    }
}
