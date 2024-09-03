using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Funnel
{
    public class Perception
    {
        private FireRate _fireRate;
        private HitPoint _hitPoint;
        private OverwriteOrder _overwriteOrder;

        public Perception(RequiredRef requiredRef)
        {
            Ref = requiredRef;
        }

        private RequiredRef Ref { get; set; }

        /// <summary>
        /// 初期化。Startのタイミングで呼ぶ想定。
        /// </summary>
        public void InitializeOnStart()
        {
            // 攻撃間隔、体力、命令
            _fireRate = new FireRate(Ref);
            _hitPoint = new HitPoint(Ref);
            _overwriteOrder = new OverwriteOrder(Ref.BlackBoard.Name);
        }

        /// <summary>
        /// 値を更新。
        /// </summary>
        public void Update()
        {
            Calculate();
            Overwrite();
        }

        // 必要な値を計算し、黒板に書き込む。
        private void Calculate()
        {
            BlackBoard bb = Ref.BlackBoard;

            // 自身からボスへのベクトルを黒板に書き込む。
            Vector3 bdir = Ref.Boss.transform.position - Ref.Transform.position;
            bb.BossDirection = bdir.normalized;
            bb.BossSqrDistance = bdir.sqrMagnitude;

            // 自身からプレイヤーへのベクトルを黒板に書き込む。
            Vector3 pdir = Ref.Player.position - Ref.Transform.position;
            bb.PlayerDirection = pdir.normalized;
            bb.PlayerSqrDistance = pdir.sqrMagnitude;

            // 攻撃タイミングを更新。
            _fireRate.UpdateIfAttacked();

            // 体力を更新。
            _hitPoint.Update();
        }

        // 黒板の値をボスからの命令で上書きする。
        private void Overwrite()
        {
            BlackBoard bb = Ref.BlackBoard;

            bool isDelete = bb.CurrentState == StateKey.Delete;
            if (isDelete) return;

            foreach (EnemyOrder order in _overwriteOrder.ForEach())
            {
                EnemyOrder.Type t = order.OrderType;

                if (t == EnemyOrder.Type.FunnelExpand) ExpandOrder();
                else if (t == EnemyOrder.Type.FunnelFireEnable) FireEnable();
                else if (t == EnemyOrder.Type.FunnelFireDisable) FireDisable();
            }

            // 展開以外の命令一覧。
            void FireEnable() => Ref.BlackBoard.IsFireEnabled = true;
            void FireDisable() => Ref.BlackBoard.IsFireEnabled = false;
        }

        // ボス本体の周囲に展開させるための命令を黒板に書き込む。
        private void ExpandOrder()
        {
            // 展開する度に体力を最大値に戻す。
            _hitPoint.ResetToMax();

            Trigger expand = Ref.BlackBoard.Expand;

            if (expand.IsWaitingExecute()) return;
            else expand.Order();

            float x = Ref.FunnelParams.Expand.Side;
            float y = Ref.FunnelParams.Expand.Height;
            float z = Ref.FunnelParams.Expand.Offset;
            Ref.BlackBoard.ExpandOffset = new Vector3(x, y, z);
        }

        /// <summary>
        /// ダメージ処理。
        /// </summary>
        public void Damage(int value, string weapon) => _hitPoint.Damage(value, weapon);

        /// <summary>
        /// EnemyManager以外から呼び出し。
        /// </summary>
        public void Order(EnemyOrder.Type type) => _overwriteOrder.Buffer(type);
    }
}