using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 外部から敵をコントロールするために、Perception層での黒板への書き込みを上書きする。
    /// 命令はUpdateのタイミングで処理するためにキューイングする。
    /// </summary>
    public class OverrideOrder
    {
        // 同フレーム内で処理できる命令の最大数。
        const int OrderCapacity = 3;

        private BlackBoard _blackBoard;

        // 予め命令用のインスタンスをプールしておき、命令をする際はプールしておいたインスタンスにコピーする。
        // コピーされたインスタンスをプールからキューに移して処理、その後またプールに戻す。
        // 命令は参照型なので、キューイングした後に同フレーム内で命令が書き換わってしまうのを防ぐ。
        private Stack<EnemyManager.Order> _pool;
        private Queue<EnemyManager.Order> _buffer;

        public OverrideOrder(BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
            _pool = new Stack<EnemyManager.Order>();
            _buffer = new Queue<EnemyManager.Order>();

            for (int i = 0; i < OrderCapacity; i++)
            {
                _pool.Push(new EnemyManager.Order());
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
                EnemyManager.Order order = _buffer.Dequeue();
                // プレイヤーを発見させる。
                if (order.OrderType == EnemyManager.Order.Type.PlayerDetect)
                {
                    _blackBoard.IsPlayerDetected = true;
                }
                // 攻撃させる。
                else if (order.OrderType == EnemyManager.Order.Type.Attack)
                {
                    _blackBoard.OrderedAttackTrigger = true;
                }
                // ポーズ
                else if (order.OrderType == EnemyManager.Order.Type.Pause)
                {
                    _blackBoard.IsOrderedPause = true;
                }
                // ポーズ解除
                else if (order.OrderType == EnemyManager.Order.Type.Resume)
                {
                    _blackBoard.IsOrderedPause = false;
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
            _blackBoard.OrderedAttackTrigger = false;
        }

        /// <summary>
        /// 命令をバッファに追加する。
        /// 次のUpdateのタイミングで処理される。
        /// </summary>
        public void Buffer(EnemyManager.Order order)
        {
            if (_pool.TryPop(out EnemyManager.Order o))
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
        public void Buffer(EnemyManager.Order.Type orderType)
        {
            if (_pool.TryPop(out EnemyManager.Order o))
            {
                o.OrderType = orderType;
                _buffer.Enqueue(o);
            }
            else Debug.LogWarning($"敵の命令がキャパオーバー: {_blackBoard.Name}");
        }
    }
}
