using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "PlayerData/CreatePlayerWeaponParam")]
public class PlayerWeaponParams : ScriptableObject
{
    public string WeaponName => _weaponName;
    public float ShotRate => _shotRate;
    public int MagazineSize => _magazineSize;
    public int ShotDamage => _shotDamege;

    [Header("===武器の基本設定===")]
    [Header("武器の名前")]
    [SerializeField] protected string _weaponName;
    [Header("連射速度")]
    [SerializeField] protected float _shotRate;
    [Header("マガジンのサイズ")]
    [SerializeField] protected int _magazineSize;
    [Header("与えるダメージ量")]
    [SerializeField] protected int _shotDamege;
}

