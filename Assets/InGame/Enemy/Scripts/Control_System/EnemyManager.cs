using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Enemy.Control
{
    public class EnemyManager : MonoBehaviour
    {
        /// <summary>
        /// シーケンス単位での制御をする際に使用。
        /// 命名はChaseSequenceControllerクラスにシリアライズされているそれに準じている。
        /// </summary>
        public enum Sequence
        {
            None,           
            Tutorial,    // 追跡:最初に敵が1体だけ出てくる。
            MultiBattle, // 追跡:乱戦中に味方機が登場。
        }

        // シーン上の敵を登録する用のメッセージ。
        private struct RegisterMessage
        {
            public EnemyController Enemy;
        }

        // シーン上の敵を登録解除する用のメッセージ。
        private struct ReleaseMessage
        {
            public EnemyController Enemy;
        }

        // 登録された敵。
        private HashSet<EnemyController> _enemies = new HashSet<EnemyController>();
        // 登録された敵に対して命令。
        // 命令の処理を呼び出す度に書き換えて使いまわす。
        private EnemyOrder _order = new EnemyOrder();

        private void Awake()
        {
            MessageBroker.Default.Receive<RegisterMessage>()
                .Subscribe(msg => _enemies.Add(msg.Enemy)).AddTo(this);
            MessageBroker.Default.Receive<ReleaseMessage>()
                .Subscribe(msg => _enemies.Remove(msg.Enemy)).AddTo(this);
        }

        private void Update()
        {
#if false
            if (Input.GetKeyDown(KeyCode.Z))
            {
                DetectPlayer(Sequence.MultiBattle);
            }
#endif
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
        /// 引数で指定したシーケンスに登場する敵が全員死亡しているかを判定する。
        /// </summary>
        public bool IsAllDefeated(Sequence sequence)
        {
            return _enemies.Where(e => e.Params.Sequence == sequence).All(e => !e.BlackBoard.IsAlive);
        }

        /// <summary>
        /// 引数で指定したシーケンスの敵を全て撃破する。
        /// </summary>
        public void DefeatThemAll(Sequence sequence)
        {
            foreach (EnemyController e in _enemies)
            {
                if (e.Params.Sequence == sequence) e.Damage(int.MaxValue, "");
            }
        }

        /// <summary>
        /// 引数で指定したシーケンスに登場する敵全員に対して
        /// 画面上に出現、プレイヤーに接近するように命令する。
        /// </summary>
        public void DetectPlayer(Sequence sequence)
        {
            // 命令をプレイヤー検出に書き換え。
            _order.OrderType = EnemyOrder.Type.PlayerDetect;

            // シーケンスを指定して命令。
            foreach (EnemyController e in _enemies)
            {
                if (e.Params.Sequence == sequence) e.Order(_order);
            }
        }

        /// <summary>
        /// 敵を登録する。
        /// </summary>
        public static void Register(EnemyController enemy)
        {
            MessageBroker.Default.Publish(new RegisterMessage { Enemy = enemy });
        }

        /// <summary>
        /// 敵の登録を解除する。
        /// </summary>
        public static void Release(EnemyController enemy)
        {
            MessageBroker.Default.Publish(new ReleaseMessage { Enemy = enemy });
        }
    }
}