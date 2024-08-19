using System.Collections.Generic;
using UnityEngine;

namespace Enemy
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
        private Queue<EnemyOrder > _buffer;

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
            // 黒板に書き込み -> 命令をクリアしてプールに戻す。
            while (_buffer.Count > 0)
            {
                EnemyOrder order = _buffer.Dequeue();
                WriteToBlackBoard(order);
                order.Clear();
                _pool.Push(order);
            }
        }

        // 命令に応じて黒板に書き込む。
        private void WriteToBlackBoard(EnemyOrder order)
        {
            EnemyOrder.Type t = order.OrderType;

            if (t == EnemyOrder.Type.PlayerDetect) { PlayerDetect(); SpawnPoint(order.Point); }
            else if (t == EnemyOrder.Type.Attack) { AttackTrigger(); }
            else if (t == EnemyOrder.Type.Pause) { Pause(true); }
            else if (t == EnemyOrder.Type.Resume) { Pause(false); }
            else if (t == EnemyOrder.Type.BossStart) { Die(); }
            else if (t == EnemyOrder.Type.QteStartTargeted) { Qte(run: true, tgt: true); AttackTrigger(); }
            else if (t == EnemyOrder.Type.QteStartUntargeted) { Qte(run: true, tgt: false); }
            else if (t == EnemyOrder.Type.QteSuccessTargeted) { Qte(run: false, tgt: false); Die(); }
            else if (t == EnemyOrder.Type.QteSuccessUntargeted) { Qte(run: false, tgt: false); }
            else if (t == EnemyOrder.Type.QteFailureTargeted) { Qte(run: false, tgt: false); }
            else if (t == EnemyOrder.Type.QteFailureUntargeted) { Qte(run: false, tgt: false); }

            // 黒板に書き込む命令一覧。
            void PlayerDetect() { _blackBoard.IsPlayerDetect = true; }
            void SpawnPoint(Vector3? p) { _blackBoard.SpawnPoint = p; }
            void AttackTrigger() { _blackBoard.OrderedAttack = Trigger.Ordered; }
            void Pause(bool b) { _blackBoard.IsPause = b; }
            void Die() { _blackBoard.Hp = 0; _blackBoard.IsDying = true; }
            void Qte(bool run, bool tgt) { _blackBoard.IsQteRunning = run; _blackBoard.IsQteTargeted = tgt; }
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
            // 既に画面から削除されている場合は命令を無効化。
            if (_blackBoard.IsCleanupReady) { order = null; return false; }

            if (_pool.TryPop(out order)) return true;
            else
            {
                Debug.LogWarning($"敵の命令がキャパオーバー: {_blackBoard.Name}");
                return false;
            }
        }
    }
}
