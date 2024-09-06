﻿using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageSE
{
    /// <summary>
    /// 敵、ボス、NPC、ファンネルで共通のダメージ音再生。
    /// </summary>
    public static void Play(Vector3 position, string source)
    {
        string seName = string.Empty;
        if (source == Const.PlayerRifleWeaponName) seName = "SE_Damage_02";
        else if (source == Const.PlayerLauncherWeaponName) seName = "SE_Damage_01";
        else if (source == Const.PlayerMeleeWeaponName) seName = "SE_PileBunker_Hit";
        else if (source == Const.PlayerMissileWeaponName) seName = "SE_Missile_Hit";

        if (seName != null) AudioWrapper.PlaySE(position, seName);
    }
}
