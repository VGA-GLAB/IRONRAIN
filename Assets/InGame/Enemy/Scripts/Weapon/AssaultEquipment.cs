namespace Enemy
{
    public class AssaultEquipment : RangeEquipment
    {
        protected override void OnShoot()
        {
            // 発射音
            AudioWrapper.PlaySE("SE_AssaultRifle");
        }
    }
}
