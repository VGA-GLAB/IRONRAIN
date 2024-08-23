using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel
{
    public class FireRate
    {
        // 攻撃タイミングがリストの要素になっており、攻撃時間が来たら添え字を更新する。
        private IReadOnlyList<float> _timing;
        private int _index;
        private float _nextTime;

        public FireRate(RequiredRef requiredRef)
        {
            Ref = requiredRef;

            float rate = requiredRef.FunnelParams.FireRate;
            Initialize(rate);
        }

        private RequiredRef Ref { get; }

        // 攻撃タイミングを初期化
        private void Initialize(float rate)
        {
            _timing = TimingFromRate(rate);

            // 現在の時間からn秒後を最初の攻撃タイミングとして設定。
            _index = 0;
            _nextTime = Time.time + _timing[_index];
        }

        // 設定したパラメータを基に、一定間隔の攻撃タイミングを作成。
        private IReadOnlyList<float> TimingFromRate(float rate)
        {
            List<float> timing = new List<float> { rate };

            return timing;
        }

        /// <summary>
        /// 攻撃を行った場合は攻撃タイミングを更新し、黒板に書き込む。
        /// </summary>
        public void UpdateIfAttacked()
        {
            // 実際に弾が発射もしくは刀を振ったタイミングではなく、
            // ステート側で攻撃の処理を行ったタイミングから次の攻撃タイミングを計算している。
            bool isCooldown = Time.time <= _nextTime;
            bool isWaiting = Ref.BlackBoard.Attack.IsWaitingExecute();
            if (isCooldown || isWaiting) return;
            else Ref.BlackBoard.Attack.Order();

            _index++;
            _index %= _timing.Count;

            // 1つ前のタイミングとの差を取ることで、次のタイミングまでの時間を計算。
            float t = _timing[_index];
            if (_index > 0) t -= _timing[_index - 1];

            _nextTime = Time.time + t;
        }
    }
}
