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
        const int OrderCapacity = 6;

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
                // プレイヤーを発見させる。
                if (order.OrderType == EnemyOrder.Type.PlayerDetect)
                {
                    _blackBoard.IsOrderedPlayerDetect = true;
                    _blackBoard.OrderedSpawnPoint = order.Point;
                }
                // 攻撃させる。
                else if (order.OrderType == EnemyOrder.Type.Attack)
                {
                    _blackBoard.OrderedAttackTrigger = true;
                }
                // ポーズ
                else if (order.OrderType == EnemyOrder.Type.Pause)
                {
                    _blackBoard.IsOrderedPause = true;
                }
                // ポーズ解除
                else if (order.OrderType == EnemyOrder.Type.Resume)
                {
                    _blackBoard.IsOrderedPause = false;
                }
                // ボス戦開始
                else if (order.OrderType == EnemyOrder.Type.BossStart)
                {
                    // HitPointクラスで、プレイヤーを検知していない状態の場合はダメージが入らないようにしている。
                    // 状態にかかわらず死亡させたいので、HPを直接書きかえる。
                    _blackBoard.Hp = 0;
                    _blackBoard.IsDying = true;
                }
                // 自身を対象としてQTE開始
                else if (order.OrderType == EnemyOrder.Type.QteStartTargeted)
                {
                    //
                }
                // 自身以外を対象としてQTE開始
                else if (order.OrderType == EnemyOrder.Type.QteStartUntargeted)
                {
                    //
                }
                // 自身を対象としてQTE成功
                else if (order.OrderType == EnemyOrder.Type.QteSuccessTargeted)
                {
                    // HitPointクラスで、プレイヤーを検知していない状態の場合はダメージが入らないようにしている。
                    // 状態にかかわらず死亡させたいので、HPを直接書きかえる。
                    _blackBoard.Hp = 0;
                    _blackBoard.IsDying = true;
                }
                // 自身以外を対象としてQTE成功
                else if (order.OrderType == EnemyOrder.Type.QteSuccessUntargeted)
                {
                    //
                }
                // 自身を対象としてQTE失敗
                else if (order.OrderType == EnemyOrder.Type.QteFailureTargeted)
                {
                    //
                }
                // 自身以外を対象としてQTE失敗
                else if (order.OrderType == EnemyOrder.Type.QteFailureUntargeted)
                {
                    //
                }

                // 命令をクリアしてプールに戻す。
                order.Clear();
                _pool.Push(order);
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

        /// <summary>
        /// 命令をバッファに追加する。
        /// 次のUpdateのタイミングで処理される。
        /// </summary>
        public void Buffer(EnemyOrder order)
        {
            if (_pool.TryPop(out EnemyOrder o))
            {
                o.OrderType = order.OrderType;
                o.Point = order.Point;
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
