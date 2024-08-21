using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss
{
    /// <summary>
    /// 攻撃タイミングを管理する。
    /// </summary>
    public class FireRate
    {
        private BlackBoard _blackBoard;
        private Equipment _meleeEquip;
        private RangeEquipment _rangeEquip;

        // 攻撃タイミングがリストの要素になっており、攻撃時間が来たら添え字を更新する。
        private IReadOnlyList<float> _rangeTiming;
        private IReadOnlyList<float> _meleeTiming;
        private int _rangeIndex;
        private int _meleeIndex;
        private float _nextRangeTime;
        private float _nextMeleeTime;

        public FireRate(RequiredRef requiredRef)
        {
            _blackBoard = requiredRef.BlackBoard;
            _meleeEquip = requiredRef.MeleeEquip;
            _rangeEquip = requiredRef.RangeEquip;
            _rangeTiming = InitRangeTiming(requiredRef.BossParams);
            _meleeTiming = InitMeleeTiming(requiredRef.BossParams);
        }

        // 遠距離攻撃タイミングを初期化
        private IReadOnlyList<float> InitRangeTiming(BossParams bossParams)
        {
            List<float> timing = new List<float>();

            if (bossParams.RangeAttackConfig.UseInputBuffer &&
                bossParams.RangeAttackConfig.InputBufferAsset)
            {
                // テキストファイルの文字列から攻撃タイミングを作成
                string text = bossParams.RangeAttackConfig.InputBufferAsset.ToString();
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
                timing.Add(bossParams.RangeAttackConfig.Rate);
            }

            // 最初の攻撃タイミングを設定
            _nextRangeTime = Time.time + timing[_rangeIndex];

            return timing;
        }

        // 近接攻撃のタイミングを初期化
        private IReadOnlyList<float> InitMeleeTiming(BossParams bossParams)
        {
            // 一定間隔で攻撃
            List<float> timing = new List<float>
            {
                bossParams.MeleeAttackConfig.Rate
            };

            // 最初の攻撃タイミングを設定
            _nextMeleeTime = Time.time + timing[_meleeIndex];

            return timing;
        }

        /// <summary>
        /// 攻撃を行った場合は攻撃タイミングを更新する。
        /// </summary>
        public void UpdateIfAttacked()
        {
            // 実際に弾が発射もしくは刀を振ったタイミングではなく、
            // ステート側で攻撃の処理を行ったタイミングから次の攻撃タイミングを計算している。

            if (_nextRangeTime < Time.time && _blackBoard.RangeAttack.Order())
            {
                _rangeIndex++;
                _rangeIndex %= _rangeTiming.Count;

                // 次のタイミングまでの時間
                float t = _rangeTiming[_rangeIndex];
                if (_rangeIndex > 0) t -= _rangeTiming[_rangeIndex - 1];

                _nextRangeTime = Time.time + t;
            }

            // 最後に近接攻撃したタイミングが次の攻撃タイミングより後の場合
            if (_nextMeleeTime < Time.time && _blackBoard.MeleeAttack.Order())
            {
                _meleeIndex++;
                _meleeIndex %= _meleeTiming.Count;

                // 次のタイミングまでの時間
                float t = _meleeTiming[_meleeIndex];
                if (_meleeIndex > 0) t -= _meleeTiming[_meleeIndex - 1];

                _nextMeleeTime = Time.time + t;
            }
        }
    }
}