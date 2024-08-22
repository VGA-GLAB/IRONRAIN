using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel
{
    public class HitPoint
    {
        // Updateで黒板に反映し、毎フレームクリアされる。
        private int _damage;
        private string _damageSource;

        public HitPoint(RequiredRef requiredRef)
        {
            Ref = requiredRef;

            // 体力の初期値
            Ref.BlackBoard.Hp = Ref.FunnelParams.MaxHp;
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

        // ダメージ耐性。常に無敵状態。
        private bool IsArmor(string weaponName)
        {
            Armor armor = Ref.FunnelParams.Armor;
            return Enemy.HitPoint.IsArmor(weaponName, armor);
        }
    }
}
