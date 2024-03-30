using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 攻撃タイミングを管理する。
    /// </summary>
    public class FireRate
    {
        private EnemyParams _params;
        private BlackBoard _blackBoard;
        private IReadOnlyList<float> _timing;

        // 攻撃する度にこの値を更新し、次の攻撃タイミングを計算する。
        private int _index;

        public FireRate(EnemyParams enemyParams, BlackBoard blackBoard)
        {
            _params = enemyParams;
            _blackBoard = blackBoard;
            _timing = InitTiming(enemyParams);
        }

        // 攻撃タイミングを初期化
        private IReadOnlyList<float> InitTiming(EnemyParams enemyParams)
        {
            List<float> timing = new List<float>();

            if (enemyParams.Tactical.UseInputBuffer &&
                enemyParams.Tactical.InputBufferAsset)
            {
                // テキストファイルの文字列から攻撃タイミングを作成
                string text = enemyParams.Tactical.InputBufferAsset.ToString();
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
                timing.Add(enemyParams.Tactical.AttackRate);
            }

            // 最初の攻撃タイミングを設定
            _blackBoard.NextAttackTime = Time.time + timing[_index];

            return timing;
        }

        /// <summary>
        /// 攻撃タイミングを更新
        /// </summary>
        public void NextTiming()
        {
            // 最後に攻撃したタイミングが次の攻撃タイミングより前の場合はそのまま
            if (_blackBoard.LastAttackTime <= _blackBoard.NextAttackTime) return;

            _index++;
            _index %= _timing.Count;

            // 次のタイミングまでの時間
            float t = _timing[_index];
            if (_index > 0) t -= _timing[_index - 1];

            _blackBoard.NextAttackTime = Time.time + t;

            // レベルの調整
            // 攻撃速度を上げるには次の攻撃タイミングまでの時間を減らす必要があるので符号を反転する。
            float order = -_blackBoard.LevelAdjust.FireRate * t;
            _blackBoard.NextAttackTime += order;
        }
    }
}
