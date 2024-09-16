using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageSE
{
    /// <summary>
    /// 敵のダメージ音再生。
    /// </summary>
    public static void PlayEnemy(Vector3 position, string source)
    {
        string seName = string.Empty;
        if (source == Const.PlayerRifleWeaponName) seName = "SE_Enemy_Damage_02";
        else if (source == Const.PlayerLauncherWeaponName) seName = "SE_Enemy_Damage_01";
        else if (source == Const.PlayerMeleeWeaponName) seName = "SE_PileBunker_Hit";
        else if (source == Const.PlayerMissileWeaponName) seName = "SE_Missile_Hit";

        if (seName != string.Empty)
        {
            AudioWrapper.PlaySE(position, seName);
        }
    }

    /// <summary>
    /// ボスのダメージ音再生。
    /// </summary>
    public static void PlayBoss(Vector3 position, string source)
    {
        string seName = string.Empty;
        if (source == Const.PlayerRifleWeaponName) seName = "SE_Boss_Damage_02";
        else if (source == Const.PlayerLauncherWeaponName) seName = "SE_Boss_Damage_01";
        else if (source == Const.PlayerMeleeWeaponName) seName = "SE_PileBunker_Hit";
        else if (source == Const.PlayerMissileWeaponName) seName = "SE_Missile_Hit";

        if (seName != string.Empty)
        {
            AudioWrapper.PlaySE(position, seName);
        }
    }
}
