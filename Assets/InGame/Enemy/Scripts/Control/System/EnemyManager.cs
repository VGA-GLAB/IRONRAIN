using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Enemy.Control
{
    public class EnemyManager : MonoBehaviour
    {
        // シーン上の敵を登録する用のメッセージ。
        private struct Message
        {
            // 登録と解除どちらを行うか指定する。
            public enum ControlMode { Register, Release };

            public EnemyController Enemy;
            public ControlMode Mode;
        }

        private HashSet<EnemyController> _enemies = new HashSet<EnemyController>();

        private void Awake()
        {
            MessageBroker.Default.Receive<Message>().Subscribe(OnMessageReceived).AddTo(this);
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
