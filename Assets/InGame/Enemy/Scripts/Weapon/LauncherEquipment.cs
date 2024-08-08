namespace Enemy
{
    public class LauncherEquipment : RangeEquipment
    {
        protected override void OnShoot()
        {
            // 発射音
            AudioWrapper.PlaySE("SE_RocketLauncher");
        }
    }
}
