using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "PlayerData/CreatePlayerWeaponParam")]
public class PlayerWeaponParams : ScriptableObject
{
    public float ShotRate => _shotRate;
    public int MagazineSize => _magazineSize;
    public int ShotDamage => _shotDamege;

    [Header("===����̊�{�ݒ�===")]
    [Header("�A�ˑ��x")]
    [SerializeField] protected float _shotRate;
    [Header("�}�K�W���̃T�C�Y")]
    [SerializeField] protected int _magazineSize;
    [Header("�^����_���[�W��")]
    [SerializeField] protected int _shotDamege;
}

