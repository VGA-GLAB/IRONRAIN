using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss
{
    public class FireRate
    {
        // 攻撃タイミングがリストの要素になっており、攻撃時間が来たら添え字を更新する。
        private IReadOnlyList<float> _rangeTiming;
        private IReadOnlyList<float> _meleeTiming;
        private int _rangeIndex;
        private int _meleeIndex;
        private float _nextRangeTime;
        private float _nextMeleeTime;

        public FireRate(RequiredRef requiredRef)
        {
            Initialize(requiredRef.BossParams.RangeAttackConfig);
            Initialize(requiredRef.BossParams.MeleeAttackConfig);

            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        // 遠距離攻撃タイミングを初期化
        private void Initialize(RangeAttackSettings settings)
        {
            bool useInputBuffer = settings.UseInputBuffer;
            bool isAssigned = settings.InputBufferAsset != null;
            if (useInputBuffer && isAssigned)
            {
                _rangeTiming = TimingFromAsset(settings);
            }
            else
            {
                _rangeTiming = TimingFromRate(settings);
            }

            // 現在の時間からn秒後を最初の攻撃タイミングとして設定。
            _rangeIndex = 0;
            _nextRangeTime = Time.time + _rangeTiming[_rangeIndex];
        }

        // 近接攻撃タイミングを初期化
        private void Initialize(MeleeAttackSettings settings)
        {
            _meleeTiming = TimingFromRate(settings);

            // 現在の時間からn秒後を最初の攻撃タイミングとして設定。
            _meleeIndex = 0;
            _nextMeleeTime = Time.time + _meleeTiming[_meleeIndex];
        }

        // テキストファイルの文字列から遠距離攻撃タイミングを作成
        private static IReadOnlyList<float> TimingFromAsset(RangeAttackSettings settings)
        {
            List<float> timing = new List<float>();

            string text = settings.InputBufferAsset.ToString();
            foreach (string s in text.Split("\n"))
            {
                if (s == "") continue;

                if (float.TryParse(s, out float f)) timing.Add(f);
                else Debug.LogWarning($"攻撃タイミングの初期化、float型に変換できない値: {s}");
            }

            return timing;
        }

        // 設定したパラメータを基に、一定間隔の遠距離攻撃タイミングを作成。
        private static IReadOnlyList<float> TimingFromRate(RangeAttackSettings settings)
        {
            return TimingFromRate(settings.Rate);
        }

        // 設定したパラメータを基に、一定間隔の近接攻撃タイミングを作成。
        private static IReadOnlyList<float> TimingFromRate(MeleeAttackSettings settings)
        {
            return TimingFromRate(settings.Rate);
        }

        // リストに詰めて返す。
        private static IReadOnlyList<float> TimingFromRate(float rate)
        {
            List<float> timing = new List<float> { rate };
            return timing;
        }

        /// <summary>
        /// 攻撃を行った場合は攻撃タイミングを更新する。
        /// </summary>
        public void UpdateIfAttacked()
        {
            // 実際に弾が発射もしくは刀を振ったタイミングではなく、
            // ステート側で攻撃の処理を行ったタイミングから次の攻撃タイミングを計算している。
            UpdateRangeTiming();
            UpdateMeleeTiming();
        }

        // 遠距離攻撃のタイミング更新
        private void UpdateRangeTiming()
        {
            bool isCooldown = Time.time <= _nextRangeTime;
            bool isWaiting = Ref.BlackBoard.RangeAttack.IsWaitingExecute();
            if (isCooldown || isWaiting) return;

            _rangeIndex++;
            _rangeIndex %= _rangeTiming.Count;

            // 次のタイミングまでの時間
            float t = _rangeTiming[_rangeIndex];
            if (_rangeIndex > 0) t -= _rangeTiming[_rangeIndex - 1];

            _nextRangeTime = Time.time + t;

            Ref.BlackBoard.RangeAttack.Order();
        }

        // 近接攻撃のタイミング更新
        private void UpdateMeleeTiming()
        {
            bool isCooldown = Time.time <= _nextMeleeTime;
            bool isWaiting = Ref.BlackBoard.MeleeAttack.IsWaitingExecute();
            if (isCooldown || isWaiting) return;

            _meleeIndex++;
            _meleeIndex %= _meleeTiming.Count;

            // 次のタイミングまでの時間
            float t = _meleeTiming[_meleeIndex];
            if (_meleeIndex > 0) t -= _meleeTiming[_meleeIndex - 1];

            _nextMeleeTime = Time.time + t;

            Ref.BlackBoard.MeleeAttack.Order();
        }
    }
}