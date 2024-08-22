using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// ダメージ耐性
    /// </summary>
    public enum Armor
    {
        None,       // 耐性なし
        Melee,      // 近接攻撃に耐性
        Range,      // 遠距離攻撃に耐性
        Invincible, // 無敵の人
    }

    public class HitPoint
    {
        // Updateで黒板に反映し、毎フレームクリアされる。
        private int _damage;
        private string _damageSource;

        public HitPoint(RequiredRef requiredRef)
        {
            Ref = requiredRef;

            // 体力の初期値
            Ref.BlackBoard.Hp = Ref.EnemyParams.MaxHp;
        }

        private RequiredRef Ref { get; set; }

        /// <summary>
        /// バッファを基にダメージ情報とHPを黒板に書き込む。
        /// </summary>
        public void Update()
        {
            BlackBoard bb = Ref.BlackBoard;
            bb.Damage = _damage;
            bb.DamageSource = _damageSource;

            if (_damage > 0)
            {
                bb.Hp -= _damage;
                bb.Hp = Mathf.Max(0, bb.Hp);
            }

            _damage = 0;
            _damageSource = "";
        }

        /// <summary>
        /// 外部からUpdate以外のタイミングでも呼ばれる想定。
        /// ダメージを計算し、次のUpdateのタイミングで黒板に書き込むため、バッファに保持しておく。
        /// </summary>
        public void Damage(int value, string weapon)
        {
            if (IsArmor(weapon)) value = 0;

            _damage += value;
            // 同一フレームに複数回ダメージを受けた場合、最後に処理された武器がダメージソースになる。
            _damageSource = weapon;
        }

        // ダメージ耐性
        private bool IsArmor(string weaponName)
        {
            Armor armor = Ref.EnemyParams.Common.Tactical.Armor;
            return IsArmor(weaponName, armor);
        }

        // ダメージ耐性
        public static bool IsArmor(string weaponName, Armor armor)
        {
            // 無敵
            if (armor == Armor.Invincible) return true;

            // 近接攻撃無効化
            bool isMelee = weaponName == Const.PlayerMeleeWeaponName;
            if (armor == Armor.Melee && isMelee) return true;

            // 遠距離攻撃無効化
            bool isRifle = weaponName == Const.PlayerRifleWeaponName;
            bool isLauncher = weaponName == Const.PlayerLauncherWeaponName;
            if (armor == Armor.Range && (isRifle || isLauncher)) return true;

            return false;
        }
    }
}