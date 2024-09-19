using UnityEngine;

namespace Enemy
{
    public class AssaultEquipment : RangeEquipment
    {
        protected override void OnShoot()
        {
            Vector3 p = transform.position;
            AudioWrapper.PlaySE(p, "SE_Enemy_AssaultRifle");
        }
    }
}
