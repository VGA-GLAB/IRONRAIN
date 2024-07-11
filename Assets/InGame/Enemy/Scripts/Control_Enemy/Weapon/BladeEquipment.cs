using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    public class BladeEquipment : MeleeEquipment
    {
        protected override void OnCollision()
        {
            AudioWrapper.PlaySE("SE_Sword");
        }
    }
}
