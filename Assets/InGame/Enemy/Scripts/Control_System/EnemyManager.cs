using Enemy.Control.Boss;
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
            FirstAnaunnce, // 追跡:最初に敵が1体だけ出てくる。
            Avoidance,     // 追跡:敵が先に攻撃してくる。
            Attack,        // 追跡:プレイヤーが1機倒す。
            TouchPanel,    // 追跡:敵を複数対出す。
            Lever,         // 追跡:レバー操作。(敵は関係なし？)
            QTETutorial,   // 追跡:盾持ちが出現、QTEする。
            MultiBattle,   // 追跡:乱戦中に味方機が登場。
            Purge,         // 追跡:装備パージ。(敵は関係なし？)
            Fall,          // ボス:時期が落下。
            BossStart,     // ボス:ボス戦開始。
            FirstFunnel,   // ボス:ファンネル展開
            ToggleButton,  // ボス:多重ロックオン(敵は関係なし？)
            SecondFunnel,  // ボス:ファンネル復活
            BossAgain,     // ボス:通常戦闘
            BreakLeftArm,  // ボス:プレイヤー左腕破壊。
            FirstBossQTE,  // ボス:QTE1回目
            SecondQTE,     // ボス:QTE2回目
            BossEnd,       // ボス:ボス戦終了演出。
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

        // シーン上のボスを登録する用のメッセージ。
        private struct BossRegisterMessage
        {
            public BossController Boss;
        }

        // シーン上のボスを登録解除する用のメッセージ。
        private struct BossReleaseMessage
        {
            public BossController Boss;
        }

        // シーン上のNPCを登録する用のメッセージ。
        private struct NpcRegisterMessage
        {
            public INpc NPC;
        }

        // シーン上のNPCを登録解除する用のメッセージ。
        private struct NpcReleaseMessage
        {
            public INpc NPC;
        }

        // 登録されたボス。
        private BossController _boss;
        // 登録された敵。
        private HashSet<EnemyController> _enemies = new HashSet<EnemyController>();
        // 登録されたNPC。
        private HashSet<INpc> _npcs = new HashSet<INpc>();
        // 登録された敵に対して命令。
        // 命令の処理を呼び出す度に書き換えて使いまわす。
        private EnemyOrder _order = new EnemyOrder();

        private void Awake()
        {
            // メッセージングで登録/登録解除する。
            MessageBroker.Default.Receive<RegisterMessage>().Subscribe(msg => _enemies.Add(msg.Enemy)).AddTo(this);
            MessageBroker.Default.Receive<ReleaseMessage>().Subscribe(msg => _enemies.Remove(msg.Enemy)).AddTo(this);
            MessageBroker.Default.Receive<BossRegisterMessage>().Subscribe(msg => _boss = msg.Boss).AddTo(this);
            MessageBroker.Default.Receive<BossReleaseMessage>().Subscribe(msg => _boss = null).AddTo(this);
            MessageBroker.Default.Receive<NpcRegisterMessage>().Subscribe(msg => _npcs.Add(msg.NPC)).AddTo(this);
            MessageBroker.Default.Receive<NpcReleaseMessage>().Subscribe(msg => _npcs.Remove(msg.NPC)).AddTo(this);
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
        /// ボス戦を開始する。
        /// </summary>
        public void BossStart()
        {
            // 命令をボス戦開始に書き換え。
            _order.OrderType = EnemyOrder.Type.BossStart;

            // ボスに命令
            if (_boss != null) _boss.Order(_order);

            // 全シーケンスの敵に対して命令
            foreach (EnemyController e in _enemies) e.Order(_order);
        }

        /// <summary>
        /// ボス戦の終盤、プレイヤーの左腕を破壊するシーケンス開始に合わせて呼ぶ。
        /// </summary>
        public void BreakLeftArm()
        {
            // 命令をプレイヤーの左腕を破壊するに切り替え。
            _order.OrderType = EnemyOrder.Type.BreakLeftArm;

            // ボスに命令
            if (_boss != null) _boss.Order(_order);
        }

        /// <summary>
        /// BreakLeftArmメソッド呼び出し後、1回目のQTEを行う。
        /// </summary>
        public void BossFirstQte()
        {
            // 命令をQTEの1回目に切り替え。
            _order.OrderType = EnemyOrder.Type.BossFirstQTE;

            // ボスに命令
            if (_boss != null) _boss.Order(_order);
        }

        /// <summary>
        /// ボス戦の終盤、プレイヤーの左腕を破壊するシーケンス開始に合わせて呼ぶ。
        /// </summary>
        public void BossSecondQte()
        {
            // 命令を2回目のQTEに切り替え。
            _order.OrderType = EnemyOrder.Type.BossSecondQTE;

            // ボスに命令
            if (_boss != null) _boss.Order(_order);
        }

        /// <summary>
        /// シーケンスを指定してNPCのイベントを実行。
        /// </summary>
        public void PlayNpcEvent(Sequence sequence)
        {
            foreach (INpc npc in _npcs)
            {
                if (npc.Sequence == sequence) npc.Play();
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
        /// ボスを登録する。
        /// </summary>
        public static void Register(BossController boss)
        {
            MessageBroker.Default.Publish(new BossRegisterMessage { Boss = boss });
        }

        /// <summary>
        /// NPCを登録する。
        /// </summary>
        public static void Register(INpc npc)
        {
            MessageBroker.Default.Publish(new NpcRegisterMessage { NPC = npc });
        }

        /// <summary>
        /// 敵の登録を解除する。
        /// </summary>
        public static void Release(EnemyController enemy)
        {
            MessageBroker.Default.Publish(new ReleaseMessage { Enemy = enemy });
        }

        /// <summary>
        /// ボスの登録を解除する。
        /// </summary>
        public static void Release(BossController boss)
        {
            MessageBroker.Default.Publish(new BossReleaseMessage { Boss = boss });
        }

        /// <summary>
        /// NPCの登録を解除する。
        /// </summary>
        public static void Release(INpc npc)
        {
            MessageBroker.Default.Publish(new NpcReleaseMessage { NPC = npc });
        }
    }
}