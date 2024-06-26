using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
{
    /// <summary>
    /// HPの変化を管理する。
    /// 攻撃で受けたダメージの反映、瀕死と死亡の状態フラグもここ。
    /// </summary>
    public class HitPoint
    {
        private BossParams _params;
        private BlackBoard _blackBoard;

        // Updateで黒板に反映し、毎フレームクリアされる。
        private Queue<DamageBuffer> _buffer;

        public HitPoint(BossParams bossParams, BlackBoard blackBoard)
        {
            _params = bossParams;
            _blackBoard = blackBoard;
            _buffer = new Queue<DamageBuffer>();
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
                if (IsArmor(damage.Source))
                {
                    _blackBoard.DamageSource = damage.Source;
                }
                else
                {
                    _blackBoard.Damage += damage.Damage;
                    _blackBoard.DamageSource = damage.Source;
                }
            }
        }

        // ダメージ耐性
        // 無効化した:true、しなかった:false
        private bool IsArmor(string _)
        {
            /* ダメージ耐性処理ｺｺ */

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