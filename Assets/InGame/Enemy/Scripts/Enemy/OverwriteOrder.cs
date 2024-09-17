using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class OverwriteOrder
    {
        string _name;

        // 命令は参照型なので、元の命令をバッファに保持すると、元を書き換えた際に反映されてしまう。
        // そのため、予めインスタンスをプールしておき、命令をする際はプールしておいたインスタンスにコピーする。
        // コピーされたインスタンスをプールからバッファに移して処理、その後またプールに戻す。
        private Stack<EnemyOrder> _pool;
        private Queue<EnemyOrder > _buffer;

        public OverwriteOrder(string name)
        {
            _name = name;
            _pool = new Stack<EnemyOrder>();
            _buffer = new Queue<EnemyOrder>();

            // 同フレーム内で処理できる命令の最大数。命令をプールしておく。
            const int OrderCapacity = 6;
            for (int i = 0; i < OrderCapacity; i++) _pool.Push(new EnemyOrder());
        }

        /// <summary>
        /// バッファに保持した命令を全て返す。
        /// 返した命令は次の命令を返す前にクリアされるので、参照を保持しないこと。
        /// </summary>
        public IEnumerable<EnemyOrder> ForEach()
        {
            while (_buffer.Count > 0)
            {
                EnemyOrder order = _buffer.Dequeue();
                yield return order;
                order.Clear();
                _pool.Push(order);
            }
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
                o.Point = order.Point;
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
                //Debug.LogWarning($"敵の命令がキャパオーバー: {_name}");
                return false;
            }
        }
    }
}
