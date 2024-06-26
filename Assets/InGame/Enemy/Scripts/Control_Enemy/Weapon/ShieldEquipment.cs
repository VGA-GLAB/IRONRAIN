using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    public class ShieldEquipment : MeleeEquipment
    {
        protected override void OnCollision()
        {
            AudioWrapper.PlaySE("SE_Shield");
        }
    }
}
