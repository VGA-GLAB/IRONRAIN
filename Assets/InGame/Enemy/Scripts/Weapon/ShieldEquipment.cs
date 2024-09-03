using UnityEngine;

namespace Enemy
{
    public class ShieldEquipment : MeleeEquipment
    {
        protected override void OnCollision()
        {
            Vector3 p = transform.position;
            AudioWrapper.PlaySE(p, "SE_Shield");
        }
    }
}
