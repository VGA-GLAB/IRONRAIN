using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Enemy.Control
{
    public class EnemyManager : MonoBehaviour
    {
        // シーケンスの判定に使用。
        public enum Sequence
        {
            None,           
            Tutorial,       // チュートリアル
            MultiBattle,    // 追跡:乱戦中に味方機が登場
        }

        // 登録された敵への命令。
        public class Order
        {
            public enum Type 
            {
                None,
                PlayerDetect, // プレイヤーを発見状態にさせる。
                Pause,        // ポーズ。
                Resume,       // ポーズ解除。
                Attack,       // 攻撃させる。
            };

            public Type OrderType;

            public void Clear()
            {
                OrderType = Type.None;
            }
        }

        // シーン上の敵を登録する用のメッセージ。
        private struct Message
        {
            // 登録と解除どちらを行うか指定する。
            public enum ControlMode { Register, Release };

            public EnemyController Enemy;
            public ControlMode Mode;
        }

        // 登録された敵。
        private HashSet<EnemyController> _enemies = new HashSet<EnemyController>();
        // 登録された敵に対して命令。
        // 命令の処理を呼び出す度に書き換えて使いまわす。
        private Order _order = new Order();

        private void Awake()
        {
            MessageBroker.Default.Receive<Message>().Subscribe(OnMessageReceived).AddTo(this);
        }

        private void Update()
        {
#if false
            // 命令テスト
            foreach (EnemyController c in _enemies)
            {
                if (c.Params.Sequence == Sequence.MultiBattle)
                {
                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        _order.OrderType = Order.Type.PlayerDetect;
                        c.Order(_order);
                    }
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        c.Attack();
                    }
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        c.Pause();
                    }
                    if (Input.GetKeyDown(KeyCode.V))
                    {
                        c.Resume();
                    }
                }
            }
#endif
        }

        // 自身へのメッセージを受信し、登録もしくは解除。
        private void OnMessageReceived(Message msg)
        {
            if (msg.Mode == Message.ControlMode.Register) _enemies.Add(msg.Enemy);
            else if (msg.Mode == Message.ControlMode.Release) _enemies.Remove(msg.Enemy);
        }

        /// <summary>
        /// シーン上に存在する敵の数を返す。
        /// 生存中の個体のみ、死亡している場合はカウントされない。
        /// </summary>
        public int EnemyCount()
        {
            return _enemies.Where(e => e.BlackBoard.IsAlive).Count();
        }

        /// <summary>
        /// 敵を登録する。
        /// </summary>
        public static void Register(EnemyController enemy)
        {
            Publish(enemy, Message.ControlMode.Register);
        }

        /// <summary>
        /// 敵の登録を解除する。
        /// </summary>
        public static void Release(EnemyController enemy)
        {
            Publish(enemy, Message.ControlMode.Release);
        }

        // 自身にメッセージを送信する。
        private static void Publish(EnemyController enemy, Message.ControlMode mode)
        {
            Message m = new Message { Enemy = enemy, Mode = mode };
            MessageBroker.Default.Publish(m);
        }
    }
}