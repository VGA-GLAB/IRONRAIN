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
    /// 自身の状態をチェックする。
    /// </summary>
    public class ConditionCheck
    {
        private Transform _transform;
        private EnemyParams _params;
        private BlackBoard _blackBoard;

        // Updateで黒板に反映し、毎フレーム0に戻る。
        private int _damageBuffer;

        public ConditionCheck(Transform transform, EnemyParams enemyParams, BlackBoard blackBoard)
        {
            _transform = transform;
            _params = enemyParams;
            _blackBoard = blackBoard;
        }

        /// <summary>
        /// 初期値を黒板に書き込む。
        /// 各メソッドで更新する前に一度必ず呼ぶ。
        /// </summary>
        public void Setup()
        {
            _blackBoard.Name = _transform.name;

            _blackBoard.Hp = _params.Battle.MaxHp;
            _blackBoard.IsDying = false;
            _blackBoard.LifeTime = _params.Battle.LifeTime;
        }

        /// <summary>
        /// フレーム間で変化した値を黒板に書き込む。
        /// </summary>
        public void Check()
        {
            _blackBoard.Hp -= _damageBuffer;
            _blackBoard.CurrentFrameDamage = _damageBuffer;
            _blackBoard.IsDying = 1.0f * _blackBoard.Hp / _params.Battle.MaxHp <= _params.Battle.Dying;

            // プレイヤーを発見していない状態では生存時間を減らさない。
            if (_blackBoard.IsPlayerDetected) _blackBoard.LifeTime -= _blackBoard.PausableDeltaTime;

            // 反映後は必要ないので0に戻す
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
