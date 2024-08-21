using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel
{
    public class HitPoint
    {
        // Updateで黒板に反映し、毎フレームクリアされる。
        private Queue<DamageBuffer> _buffer;

        public HitPoint(RequiredRef requiredRef)
        {
            Ref = requiredRef;
            _buffer = new Queue<DamageBuffer>();
        }

        private RequiredRef Ref { get; set; }

        /// <summary>
        /// 初期値を黒板に書き込む。
        /// </summary>
        public void Init()
        {
            Ref.BlackBoard.Hp = Ref.FunnelParams.MaxHp;
        }

        /// <summary>
        /// ダメージの体力への反映、死亡状態のフラグの制御。
        /// </summary>
        public void Update()
        {
            BlackBoard bb = Ref.BlackBoard;

            if (bb.CurrentState == FSM.StateKey.Hide) return;

            // 一度初期化
            bb.Damage = 0;
            bb.DamageSource = "";

            // Damageメソッド自体が呼ばれていない場合。
            if (_buffer.Count == 0) return;

            // 受けたダメージを処理する。
            // 同一フレームに複数回ダメージを受けた場合は、
            // 一番最後に処理されたダメージを与えた武器がダメージソースとして書き込まれる。
            while (_buffer.TryDequeue(out DamageBuffer damage))
            {
                // 耐性がある、もしくはプレイヤーを検知していない状態。
                if (IsArmor(damage.Source) || !bb.IsPlayerDetect)
                {
                    bb.DamageSource = damage.Source;
                }
                else
                {
                    bb.Hp -= damage.Damage;
                    bb.Hp = Mathf.Max(0, bb.Hp);
                    bb.Damage += damage.Damage;
                    bb.DamageSource = damage.Source;
                }
            }
        }

        // ダメージ耐性
        // 無効化した:true、しなかった:false
        private bool IsArmor(string weaponName) // staticで十分
        {
            Armor armor = Ref.FunnelParams.Armor;

            // 無敵
            if (armor == Armor.Invincible) return true;

            // 近接攻撃無効化
            if (armor == Armor.Melee &&
                weaponName == Const.PlayerMeleeWeaponName) return true;

            // 遠距離攻撃無効化
            if (armor == Armor.Range &&
                (weaponName == Const.PlayerRifleWeaponName ||
                weaponName == Const.PlayerLauncherWeaponName)) return true;

            return false;
        }

        /// <summary>
        /// ダメージを受ける処理。
        /// 次のUpdateのタイミングまで黒板には書き込まない。
        /// </summary>
        public void Damage(int value, string weapon)
        {
            // 武器が空文字だった場合
            if (weapon == "") weapon = "Unknown";

            _buffer.Enqueue(new DamageBuffer() { Damage = value, Source = weapon });
        }
    }
}
