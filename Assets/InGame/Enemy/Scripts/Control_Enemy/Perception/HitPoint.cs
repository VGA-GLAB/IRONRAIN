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
    /// HPの変化を管理する。
    /// 攻撃で受けたダメージの反映、瀕死と死亡の状態フラグもここ。
    /// </summary>
    public class HitPoint
    {
        private EnemyParams _params;
        private BlackBoard _blackBoard;

        // Updateで黒板に反映し、毎フレーム0に戻る。
        private int _damageBuffer;

        public HitPoint(EnemyParams enemyParams, BlackBoard blackBoard)
        {
            _params = enemyParams;
            _blackBoard = blackBoard;
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
            // プレイヤーを検知していない状態だとダメージが入らない。
            if (_blackBoard.IsPlayerDetected)
            {
                _blackBoard.Hp -= _damageBuffer;
                _blackBoard.Hp = Mathf.Max(0, _blackBoard.Hp);
                _blackBoard.CurrentFrameDamage = _damageBuffer;
                _blackBoard.IsDying = 1.0f * _blackBoard.Hp / _params.Battle.MaxHp <= _params.Battle.Dying;
            }

            // フレーム毎のダメージなので反映後は0に戻す。
            _damageBuffer = 0;
        }

        /// <summary>
        /// ダメージを受ける処理。
        /// 次のUpdateのタイミングまで黒板には書き込まない。
        /// </summary>
        public void Damage(int value, string weapon)
        {
            // 無敵
            if (_params.Common.Tactical.Armor == Armor.Invincible) return;

            // 近接攻撃無効化
            if (_params.Common.Tactical.Armor == Armor.Melee &&
                weapon == Const.PlayerMeleeWeaponName) return;

            // 遠距離攻撃無効化
            if (_params.Common.Tactical.Armor == Armor.Range &&
                weapon == Const.PlayerRangeWeaponName) return;

            _damageBuffer += value;
        }
    }
}
