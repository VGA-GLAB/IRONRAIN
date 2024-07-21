using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
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

    /// <summary>
    /// ダメージ情報を一時的に保持する用途。
    /// </summary>
    public struct DamageBuffer
    {
        public int Damage;
        public string Source;
    }

    /// <summary>
    /// HPの変化を管理する。
    /// 攻撃で受けたダメージの反映、瀕死と死亡の状態フラグもここ。
    /// </summary>
    public class HitPoint
    {
        private EnemyParams _params;
        private BlackBoard _blackBoard;

        // Updateで黒板に反映し、毎フレームクリアされる。
        private Queue<DamageBuffer> _buffer;

        public HitPoint(EnemyParams enemyParams, BlackBoard blackBoard)
        {
            _params = enemyParams;
            _blackBoard = blackBoard;
            _buffer = new Queue<DamageBuffer>();
        }

        /// <summary>
        /// 初期値を黒板に書き込む。
        /// </summary>
        public void Init()
        {
            _blackBoard.Hp = _params.Battle.MaxHp;
            _blackBoard.IsDying = false;
        }

        /// <summary>
        /// ダメージの体力への反映、死亡状態のフラグの制御。
        /// </summary>
        public void Update()
        {
            // 一度初期化
            _blackBoard.Damage = 0;
            _blackBoard.DamageSource = "";

            // Damageメソッド自体が呼ばれていない場合。
            if (_buffer.Count == 0) return;

            // 受けたダメージを処理する。
            // 同一フレームに複数回ダメージを受けた場合は、
            // 一番最後に処理されたダメージを与えた武器がダメージソースとして書き込まれる。
            while (_buffer.TryDequeue(out DamageBuffer damage))
            {
                // 耐性がある、もしくはプレイヤーを検知していない状態。
                if (IsArmor(damage.Source) || !_blackBoard.IsPlayerDetected)
                {
                    _blackBoard.DamageSource = damage.Source;
                }
                else
                {
                    _blackBoard.Hp -= damage.Damage;
                    _blackBoard.Hp = Mathf.Max(0, _blackBoard.Hp);
                    _blackBoard.Damage += damage.Damage;
                    _blackBoard.DamageSource = damage.Source;
                    _blackBoard.IsDying = 1.0f * _blackBoard.Hp / _params.Battle.MaxHp <= _params.Battle.Dying;
                }
            }
        }

        // ダメージ耐性
        // 無効化した:true、しなかった:false
        private bool IsArmor(string weaponName)
        {
            // 無敵
            if (_params.Common.Tactical.Armor == Armor.Invincible) return true;
            
            // 近接攻撃無効化
            if (_params.Common.Tactical.Armor == Armor.Melee &&
                weaponName == Const.PlayerMeleeWeaponName) return true;

            // 遠距離攻撃無効化
            if (_params.Common.Tactical.Armor == Armor.Range &&
                weaponName == Const.PlayerAssaultRifleWeaponName) return true;

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
            Debug.Log(weapon);

            _buffer.Enqueue(new DamageBuffer() { Damage = value, Source = weapon });
        }
    }
}