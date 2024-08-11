using Enemy.Boss.FSM;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss
{
    /// <summary>
    /// 外部から敵をコントロールするために、Perception層での黒板への書き込みを上書きする。
    /// 命令はUpdateのタイミングで処理するためにキューイングする。
    /// </summary>
    public class OverrideOrder
    {
        private BlackBoard _blackBoard;

        // 予め命令用のインスタンスをプールしておき、命令をする際はプールしておいたインスタンスにコピーする。
        // コピーされたインスタンスをプールからキューに移して処理、その後またプールに戻す。
        // 命令は参照型なので、キューイングした後に同フレーム内で命令が書き換わってしまうのを防ぐ。
        private Stack<EnemyOrder> _pool;
        private Queue<EnemyOrder> _buffer;

        public OverrideOrder(RequiredRef requiredRef)
        {
            _blackBoard = requiredRef.BlackBoard;
            _pool = new Stack<EnemyOrder>();
            _buffer = new Queue<EnemyOrder>();

            // 同フレーム内で処理できる命令の最大数。命令をプールしておく。
            const int OrderCapacity = 6;
            for (int i = 0; i < OrderCapacity; i++) _pool.Push(new EnemyOrder());
        }

        /// <summary>
        /// バッファ内の命令を全て処理する。
        /// 黒板に書き込むのはこのタイミング。
        /// </summary>
        public void Update()
        {
            while (_buffer.Count > 0)
            {
                EnemyOrder order = _buffer.Dequeue();

                // 黒板に書き込み -> 命令をクリアしてプールに戻す。
                WriteToBlackBoard(order);
                order.Clear();
                _pool.Push(order);
            }
        }

        // 命令に応じて黒板に書き込む。
        private void WriteToBlackBoard(EnemyOrder order)
        {
            EnemyOrder.Type t = order.OrderType;

            if (t == EnemyOrder.Type.BossStart) { BossStart(); }
            else if (t == EnemyOrder.Type.FunnelExpand) { FunnelExpand(); }
            else if (t == EnemyOrder.Type.FunnelLaserSight) { FunnelLaserSight(); }
            else if (t == EnemyOrder.Type.BreakLeftArm) { QteStart(); QteStep(QteEventState.Step.BreakLeftArm); }
            else if (t == EnemyOrder.Type.BossFirstQTE) { QteStart(); QteStep(QteEventState.Step.FirstQte); }
            else if (t == EnemyOrder.Type.BossSecondQTE) { QteStart(); QteStep(QteEventState.Step.SecondQte); }
            else if (t == EnemyOrder.Type.BossEnd) { BossBroken(); }

            // 黒板に書き込む命令一覧。
            void BossStart() { _blackBoard.IsBossStarted = true; }
            void FunnelExpand() { _blackBoard.FunnelExpandTrigger = true; }
            void FunnelLaserSight() { _blackBoard.IsFunnelLaserSight = true; }
            void QteStart() { _blackBoard.IsQteEventStarted = true; }
            void QteStep(QteEventState.Step step) { _blackBoard.OrderdQteEventStep = step; }
            void BossBroken() { _blackBoard.IsBroken = true; }
        }

        /// <summary>
        /// 命令をバッファに追加する。
        /// 次のUpdateのタイミングで処理される。
        /// </summary>
        public void Buffer(EnemyOrder order)
        {
            if (TryRentPoolingOrder(out EnemyOrder o))
            {
                o.OrderType = order.OrderType;
                _buffer.Enqueue(o);
            }
        }

        /// <summary>
        /// 命令をバッファに追加する。
        /// 次のUpdateのタイミングで処理される。
        /// EnemyManager以外はOrderクラスのインスタンスを保持していないのでこっちで呼ぶ。
        /// </summary>
        public void Buffer(EnemyOrder.Type orderType)
        {
            if (TryRentPoolingOrder(out EnemyOrder o))
            {
                o.OrderType = orderType;
                _buffer.Enqueue(o);
            }
        }

        // プーリングされている命令を取り出す。
        private bool TryRentPoolingOrder(out EnemyOrder order)
        {
            if (_pool.TryPop(out order)) return true;
            else
            {
                Debug.LogWarning($"敵の命令がキャパオーバー: {_blackBoard.Name}");
                return false;
            }
        }

        /// <summary>
        /// LateUpdateで呼ぶことで、次のフレームを跨ぐ前にトリガー系の命令を元に戻す。
        /// </summary>
        public void ClearOrderedTrigger()
        {
            _blackBoard.FunnelExpandTrigger = false;
        }
    }
}
