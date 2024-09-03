using UnityEngine;

namespace Enemy
{
    public class LauncherEquipment : RangeEquipment
    {
        protected override void OnShoot()
        {
            // 発射音
            Vector3 p = transform.position;
            AudioWrapper.PlaySE(p, "SE_RocketLauncher");
        }
    }
}
