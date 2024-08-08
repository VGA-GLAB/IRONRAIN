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
        // 同フレーム内で処理できる命令の最大数。
        const int OrderCapacity = 6;

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

            for (int i = 0; i < OrderCapacity; i++)
            {
                _pool.Push(new EnemyOrder());
            }
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
                // プレイヤーを発見させる。
                if (order.OrderType == EnemyOrder.Type.BossStart)
                {
                    _blackBoard.IsBossStarted = true;
                }
                // ファンネル展開
                else if (order.OrderType == EnemyOrder.Type.FunnelExpand)
                {
                    _blackBoard.FunnelExpandTrigger = true;
                }
                // プレイヤーの左手破壊
                else if (order.OrderType == EnemyOrder.Type.BreakLeftArm)
                {
                    _blackBoard.IsQteEventStarted = true;
                    _blackBoard.OrderdQteEventStep = FSM.QteEventState.Step.BreakLeftArm;
                }
                // QTE1回目
                else if (order.OrderType == EnemyOrder.Type.BossFirstQTE)
                {
                    _blackBoard.IsQteEventStarted = true;
                    _blackBoard.OrderdQteEventStep = FSM.QteEventState.Step.FirstQte;
                }
                // QTE2回目
                else if (order.OrderType == EnemyOrder.Type.BossSecondQTE)
                {
                    _blackBoard.IsQteEventStarted = true;
                    _blackBoard.OrderdQteEventStep = FSM.QteEventState.Step.SecondQte;
                }
                // ボス戦終了
                else if (order.OrderType == EnemyOrder.Type.BossEnd)
                {
                    _blackBoard.IsBroken = true;
                }

                // 命令をクリアしてプールに戻す。
                order.Clear();
                _pool.Push(order);
            }
        }

        /// <summary>
        /// LateUpdateで呼ぶことで、次のフレームを跨ぐ前にトリガー系の命令を元に戻す。
        /// </summary>
        public void ClearOrderedTrigger()
        {
            _blackBoard.FunnelExpandTrigger = false;
        }

        /// <summary>
        /// 命令をバッファに追加する。
        /// 次のUpdateのタイミングで処理される。
        /// </summary>
        public void Buffer(EnemyOrder order)
        {
            if (_pool.TryPop(out EnemyOrder o))
            {
                o.OrderType = order.OrderType;
                _buffer.Enqueue(o);
            }
            else Debug.LogWarning($"敵の命令がキャパオーバー: {_blackBoard.Name}");
        }

        /// <summary>
        /// 命令をバッファに追加する。
        /// 次のUpdateのタイミングで処理される。
        /// EnemyManager以外はOrderクラスのインスタンスを保持していないのでこっちで呼ぶ。
        /// </summary>
        public void Buffer(EnemyOrder.Type orderType)
        {
            if (_pool.TryPop(out EnemyOrder o))
            {
                o.OrderType = orderType;
                _buffer.Enqueue(o);
            }
            else Debug.LogWarning($"敵の命令がキャパオーバー: {_blackBoard.Name}");
        }
    }
}
