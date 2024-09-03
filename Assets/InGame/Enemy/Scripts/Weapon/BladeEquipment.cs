using UnityEngine;

namespace Enemy
{
    public class BladeEquipment : MeleeEquipment
    {
        protected override void OnCollision()
        {
            Vector3 p = transform.position;
            AudioWrapper.PlaySE(p, "SE_Sword");
        }
    }
}
