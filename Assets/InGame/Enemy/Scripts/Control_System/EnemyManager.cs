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

        // 登録する用のメッセージ。
        private struct RegisterMessage<T>
        {
            public T Character;
        }

        // 登録解除する用のメッセージ。
        private struct ReleaseMessage<T>
        {
            public T Character;
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
            // メッセージングで敵を登録/登録解除する。
            MessageBroker.Default.Receive<RegisterMessage<EnemyController>>()
                .Subscribe(msg => _enemies.Add(msg.Character)).AddTo(this);
            MessageBroker.Default.Receive<ReleaseMessage<EnemyController>>()
                .Subscribe(msg => _enemies.Remove(msg.Character)).AddTo(this);

            // メッセージングでボスを登録/登録解除する。
            MessageBroker.Default.Receive<RegisterMessage<BossController>>()
                .Subscribe(msg => _boss = msg.Character).AddTo(this);
            MessageBroker.Default.Receive<ReleaseMessage<BossController>>()
                .Subscribe(_ => _boss = null).AddTo(this);

            // メッセージングでNPCを登録/登録解除する。
            MessageBroker.Default.Receive<RegisterMessage<INpc>>()
                .Subscribe(msg => _npcs.Add(msg.Character)).AddTo(this);
            MessageBroker.Default.Receive<ReleaseMessage<INpc>>()
                .Subscribe(msg => _npcs.Remove(msg.Character)).AddTo(this);

#if false
            // QTE開始/終了を各キャラクターに伝える。
            ProvidePlayerInformation.StartQte.Subscribe(OnQteStart).AddTo(this);
            ProvidePlayerInformation.EndQte.Subscribe(OnQteEnd).AddTo(this);

            void OnQteStart(xxx arg)
            {
                foreach (EnemyController e in _enemies)
                {
                    if (arg.ID == e.BlackBoard.ID)
                    {
                        // QTEのチュートリアルシーケンスに登場する敵に対して、自身を対象としてQTE開始を命令。
                        _order.OrderType = EnemyOrder.Type.QteStartTargeted;
                    }
                    else
                    {
                        // それ以外のシーケンスの敵に対しては、自身以外を対象としてQTE開始を命令。
                        _order.OrderType = EnemyOrder.Type.QteStartUntargeted;
                    }

                    e.Order(_order);
                }
            }

            void OnQteEnd(xxx result)
            {
                foreach (EnemyController e in _enemies)
                {
                    if (result.ID == e.BlackBoard.ID)
                    {
                        // QTEのチュートリアルシーケンスに登場する敵に対して、自身を対象としてQTE開始を命令。
                        _order.OrderType = EnemyOrder.Type.QteEndTargeted;
                    }
                    else
                    {
                        // それ以外のシーケンスの敵に対しては、自身以外を対象としてQTE開始を命令。
                        _order.OrderType = EnemyOrder.Type.QteEndUntargeted;
                    }

                    e.Order(_order);
                }
            }
#endif
        }

        /// <summary>
        /// シーケンスを指定して敵を取得。
        /// </summary>
        /// <param name="enemies">この引数に敵への参照を入れて返す。</param>
        /// <returns>敵を1体以上取得:true 取得できる敵がいない:false</returns>
        public bool TryGetEnemies(Sequence sequence, List<EnemyController> enemies)
        {
            if (enemies == null) return false;
            
            // 呼ばれる度に入れ物は空にする。
            enemies.Clear();

            if (_enemies == null) return false;

            foreach (EnemyController e in _enemies)
            {
                if (e.Params.Sequence == sequence) enemies.Add(e);
            }

            return enemies.Count > 0;
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
        /// シーケンスを指定してポーズ
        /// </summary>
        public void Pause(Sequence sequence)
        {
            // 命令をポーズに書き換え。
            _order.OrderType = EnemyOrder.Type.Pause;

            // シーケンスを指定して命令。
            foreach (EnemyController e in _enemies)
            {
                if (e.Params.Sequence == sequence) e.Order(_order);
            }
        }

        /// <summary>
        /// シーケンスを指定してポーズ解除
        /// </summary>
        public void Resume(Sequence sequence)
        {
            // 命令をポーズ解除に書き換え。
            _order.OrderType = EnemyOrder.Type.Resume;

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
        /// ボス戦でファンネル展開。
        /// </summary>
        public void FunnelExpand()
        {
            // 命令をファンネル展開に切り替え。
            _order.OrderType = EnemyOrder.Type.FunnelExpand;

            // ボスに命令
            if (_boss != null) _boss.Order(_order);
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
        /// 登録する。
        /// </summary>
        public static void Register<T>(T character)
        {
            if (!IsRegistrable<T>()) return;

            MessageBroker.Default.Publish(new RegisterMessage<T> { Character = character });
        }

        /// <summary>
        /// 登録を解除する。
        /// </summary>
        public static void Release<T>(T character)
        {
            if (!IsRegistrable<T>()) return;

            MessageBroker.Default.Publish(new ReleaseMessage<T> { Character = character });
        }

        // 登録可能な型かチェック
        private static bool IsRegistrable<T>()
        {
            if (typeof(T) == typeof(EnemyController)) return true;
            else if (typeof(T) == typeof(BossController)) return true;
            else if (typeof(T) == typeof(INpc)) return true;
            else return false;
        }
    }
}