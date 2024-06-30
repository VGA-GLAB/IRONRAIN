﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
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