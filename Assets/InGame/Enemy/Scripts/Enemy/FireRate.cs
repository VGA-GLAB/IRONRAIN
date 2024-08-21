using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 攻撃タイミングを管理する。
    /// </summary>
    public class FireRate
    {
        private BlackBoard _blackBoard;
        private Equipment _equip;

        // 攻撃タイミングがリストの要素になっており、攻撃時間が来たら添え字を更新する。
        private IReadOnlyList<float> _timing;
        private int _index;
        private float _nextTime;

        public FireRate(RequiredRef requiredRef)
        {
            _blackBoard = requiredRef.BlackBoard;
            _equip = requiredRef.Equipment;
            _timing = InitTiming(requiredRef.EnemyParams);
        }

        // 攻撃タイミングを初期化
        private IReadOnlyList<float> InitTiming(EnemyParams enemyParams)
        {
            List<float> timing = new List<float>();

            if (enemyParams.Attack.UseInputBuffer && enemyParams.Attack.InputBufferAsset != null)
            {
                // テキストファイルの文字列から攻撃タイミングを作成
                string text = enemyParams.Attack.InputBufferAsset.ToString();
                foreach (string s in text.Split("\n"))
                {
                    if (s == "") continue;

                    if (float.TryParse(s, out float f)) timing.Add(f);
                    else Debug.LogWarning($"攻撃タイミングの初期化、float型に変換できない値: {s}");
                }
            }
            else
            {
                // 一定間隔で攻撃
                timing.Add(enemyParams.Attack.Rate);
            }

            // 最初の攻撃タイミングを設定
            _nextTime = Time.time + timing[_index];

            return timing;
        }

        /// <summary>
        /// 攻撃を行った場合は攻撃タイミングを更新する。
        /// </summary>
        public void UpdateIfAttacked()
        {
            if (_blackBoard.CurrentState == StateKey.Hide) return;

            // 実際に弾が発射もしくは刀を振ったタイミングではなく、
            // ステート側で攻撃の処理を行ったタイミングから次の攻撃タイミングを計算している。
            if (Time.time <= _nextTime) return;

            if (_blackBoard.Attack.Order())
            {
                _index++;
                _index %= _timing.Count;

                // 次のタイミングまでの時間
                float t = _timing[_index];
                if (_index > 0) t -= _timing[_index - 1];

                _nextTime = Time.time + t;
            }
        }
    }
}
