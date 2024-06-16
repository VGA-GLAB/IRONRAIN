using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control.Boss
{
    /// <summary>
    /// 攻撃タイミングを管理する。
    /// </summary>
    public class FireRate
    {
        private BlackBoard _blackBoard;
        private IReadOnlyList<float> _rangeTiming;
        private IReadOnlyList<float> _meleeTiming;

        // 攻撃する度にこの値を更新し、次の攻撃タイミングを計算する。
        private int _rangeIndex;
        private int _meleeIndex;

        public FireRate(BossParams bossParams, BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
            _rangeTiming = InitRangeTiming(bossParams);
            _meleeTiming = InitMeleeTiming(bossParams);
        }

        // 遠距離攻撃タイミングを初期化
        private IReadOnlyList<float> InitRangeTiming(BossParams bossParams)
        {
            List<float> timing = new List<float>();

            if (bossParams.Battle.RangeAttackConfig.UseInputBuffer &&
                bossParams.Battle.RangeAttackConfig.InputBufferAsset)
            {
                // テキストファイルの文字列から攻撃タイミングを作成
                string text = bossParams.Battle.RangeAttackConfig.InputBufferAsset.ToString();
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
                timing.Add(bossParams.Battle.RangeAttackConfig.AttackRate);
            }

            // 最初の攻撃タイミングを設定
            _blackBoard.NextRangeAttackTime = Time.time + timing[_rangeIndex];

            return timing;
        }

        // 近接攻撃のタイミングを初期化
        private IReadOnlyList<float> InitMeleeTiming(BossParams bossParams)
        {
            // 一定間隔で攻撃
            List<float> timing = new List<float>
            {
                bossParams.Battle.MeleeAttackConfig.AttackRate
            };

            // 最初の攻撃タイミングを設定
            _blackBoard.NextMeleeAttackTime = Time.time + timing[_meleeIndex];

            return timing;
        }

        /// <summary>
        /// 攻撃を行った場合は攻撃タイミングを更新する。
        /// </summary>
        public void UpdateIfAttacked()
        {
            // 最後に遠距離攻撃したタイミングが次の攻撃タイミングより後の場合
            if (_blackBoard.NextRangeAttackTime < _blackBoard.LastRangeAttackTime)
            {
                _rangeIndex++;
                _rangeIndex %= _rangeTiming.Count;

                // 次のタイミングまでの時間
                float t = _rangeTiming[_rangeIndex];
                if (_rangeIndex > 0) t -= _rangeTiming[_rangeIndex - 1];

                _blackBoard.NextRangeAttackTime = Time.time + t;
            }

            // 最後に近接攻撃したタイミングが次の攻撃タイミングより後の場合
            if (_blackBoard.NextMeleeAttackTime < _blackBoard.LastMeleeAttackTime)
            {
                _meleeIndex++;
                _meleeIndex %= _meleeTiming.Count;

                // 次のタイミングまでの時間
                float t = _meleeTiming[_meleeIndex];
                if (_meleeIndex > 0) t -= _meleeTiming[_meleeIndex - 1];

                _blackBoard.NextMeleeAttackTime = Time.time + t;
            }
        }
    }
}
