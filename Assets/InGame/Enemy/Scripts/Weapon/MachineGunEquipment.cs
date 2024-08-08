namespace Enemy
{
    public class MachineGunEquipment : RangeEquipment
    {
        protected override void OnShoot()
        {
            // 発射音
            AudioWrapper.PlaySE("SE_AssaultRifle");
        }
    }
}
