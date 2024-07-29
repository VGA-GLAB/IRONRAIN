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
        private const int OrderCapacity = 6;

        private BlackBoard _blackBoard;

        // 予め命令用のインスタンスをプールしておき、命令をする際はプールしておいたインスタンスにコピーする。
        // コピーされたインスタンスをプールからキューに移して処理、その後またプールに戻す。
        // 命令は参照型なので、キューイングした後に同フレーム内で命令が書き換わってしまうのを防ぐ。
        private Stack<EnemyOrder> _pool;
        private Queue<EnemyOrder > _buffer;

        public OverrideOrder(BlackBoard blackBoard)
        {
            _blackBoard = blackBoard;
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

            if (t == EnemyOrder.Type.PlayerDetect)
            {
                _blackBoard.IsOrderedPlayerDetect = true;
                _blackBoard.OrderedSpawnPoint = order.Point;
            }
            else if (t == EnemyOrder.Type.Attack)
            {
                _blackBoard.OrderedAttackTrigger = true;
            }
            else if (t == EnemyOrder.Type.Pause)
            {
                _blackBoard.IsOrderedPause = true;
            }
            else if (t == EnemyOrder.Type.Resume)
            {
                _blackBoard.IsOrderedPause = false;
            }
            else if (t == EnemyOrder.Type.BossStart)
            {
                // HitPointクラスでプレイヤーを検知していない状態の場合はダメージが入らないようにしている。
                // 状態にかかわらず死亡させたいのでHPを直接書きかえる。
                _blackBoard.Hp = 0;
                _blackBoard.IsDying = true;
            }
            else if (t == EnemyOrder.Type.QteStartTargeted)
            {
                //
            }
            else if (t == EnemyOrder.Type.QteStartUntargeted)
            {
                //
            }
            else if (t == EnemyOrder.Type.QteSuccessTargeted)
            {
                // BossStartと同じ理由、同じ処理。
                _blackBoard.Hp = 0;
                _blackBoard.IsDying = true;
            }
            else if (t == EnemyOrder.Type.QteSuccessUntargeted)
            {
                //
            }
            else if (t == EnemyOrder.Type.QteFailureTargeted)
            {
                //
            }
            else if (t == EnemyOrder.Type.QteFailureUntargeted)
            {
                //
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
                Debug.LogWarning($"敵の命令がキャパオーバー: {_blackBoard.Name}");
                return false;
            }
        }

        /// <summary>
        /// 同フレームの間だけtrueになるトリガー系の命令。
        /// LateUpdateで呼ぶことで、次のフレームを跨ぐ前にfalseに戻す。
        /// </summary>
        public void ClearOrderedTrigger()
        {
            _blackBoard.OrderedAttackTrigger = false;
        }
    }
}
