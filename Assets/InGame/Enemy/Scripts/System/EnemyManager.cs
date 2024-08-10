﻿using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Enemy.Boss;
using Enemy.NPC;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        /// <summary>
        /// シーケンス単位での制御をする際に使用する列挙型。
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
            // QTE開始/終了を各キャラクターに伝える。
            ProvidePlayerInformation.StartQte.Subscribe(OnQteStart).AddTo(this);
            ProvidePlayerInformation.EndQte.Subscribe(OnQteEnd).AddTo(this);
        }

        // QTE開始
        private void OnQteStart(System.Guid guid)
        {
            foreach (EnemyController e in _enemies)
            {
                _order.OrderType = 
                    guid == e.BlackBoard.ID ? 
                    EnemyOrder.Type.QteStartTargeted : 
                    EnemyOrder.Type.QteStartUntargeted;

                e.Order(_order);
            }
        }

        // QTE終了
        private void OnQteEnd(QteResultData result)
        {
            foreach (EnemyController e in _enemies)
            {
                if (result.EnemyId == e.BlackBoard.ID)
                {
                    _order.OrderType =
                        result.ResultType == QTEResultType.Success ?
                        EnemyOrder.Type.QteSuccessTargeted :
                        EnemyOrder.Type.QteFailureTargeted;
                }
                else
                {
                    _order.OrderType =
                        result.ResultType == QTEResultType.Success ?
                        EnemyOrder.Type.QteSuccessUntargeted :
                        EnemyOrder.Type.QteFailureUntargeted;
                }

                e.Order(_order);
            }
        }

        /// <summary>
        /// シーケンスを指定して敵を取得。
        /// </summary>
        /// <param name="enemies">この引数に敵への参照を入れて返す。</param>
        /// <returns>敵を1体以上取得:true 取得できる敵がいない:false</returns>
        public bool TryGetEnemies(Sequence sequence, List<EnemyController> enemies)
        {        
            if (_enemies == null) return false;

            enemies.Clear();
            foreach (EnemyController e in EnemiesInSequence(sequence))
            {
                enemies.Add(e);
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
            return EnemiesInSequence(sequence).All(e => !e.BlackBoard.IsAlive);
        }

        /// <summary>
        /// 引数で指定したシーケンスの敵を全て撃破する。
        /// </summary>
        public void DefeatThemAll(Sequence sequence)
        {
            foreach (EnemyController e in EnemiesInSequence(sequence))
            {
                // 極大ダメージを与えて体力を0にする。
                e.Damage(int.MaxValue / 2, "");
            }
        }

        // こっちはシーン上に置いた位置からそのまま出現するため
        // 後々にSpawnメソッドを呼ぶようにSequence側を書き換える必要あり。
        /// <summary>
        /// 引数で指定したシーケンスに登場する敵全員に対して
        /// 画面上に出現、プレイヤーに接近するように命令する。
        /// </summary>
        public void DetectPlayer(Sequence sequence)
        {
            OrderToEnemies(sequence, EnemyOrder.Type.PlayerDetect);
        }

        /// <summary>
        /// 引数で指定したシーケンスに登場する敵全員に対して
        /// 画面上に出現、プレイヤーに接近するように命令する。
        /// </summary>
        /// <param name="point">出現位置</param>
        public void Spawn(Vector3 point, Sequence sequence)
        {
            // 命令をプレイヤー検出に書き換え。
            _order.OrderType = EnemyOrder.Type.PlayerDetect;

            // シーケンスの敵全体の中心を求める。
            Vector3 center = Vector3.zero;
            int length = 0;
            foreach (EnemyController e in _enemies)
            {
                if (e.Params.Sequence == sequence)
                {
                    center += e.transform.position;
                    length++;
                }
            }
            center /= length;

            // 中心点を引数の位置としてそこからのベクトルを足す。
            foreach (EnemyController e in _enemies)
            {
                if (e.Params.Sequence == sequence)
                {
                    Vector3 dist = center - e.transform.position;
                    _order.Point = point + dist;
                    
                    e.Order(_order);
                }
            }
        }

        /// <summary>
        /// シーケンスを指定してポーズ
        /// </summary>
        public void Pause(Sequence sequence)
        {
            OrderToEnemies(sequence, EnemyOrder.Type.Pause);
        }

        /// <summary>
        /// シーケンスを指定してポーズ解除
        /// </summary>
        public void Resume(Sequence sequence)
        {
            OrderToEnemies(sequence, EnemyOrder.Type.Resume);
        }

        /// <summary>
        /// ボス戦を開始する。
        /// </summary>
        public void BossStart()
        {
            OrderToBoss(EnemyOrder.Type.BossStart);
            // ↑のメソッドで命令を書き換えているため、そのまま雑魚全員に命令。
            foreach (EnemyController e in _enemies) e.Order(_order);
        }

        /// <summary>
        /// ボス戦でファンネル展開。
        /// </summary>
        public void FunnelExpand() => OrderToBoss(EnemyOrder.Type.FunnelExpand);

        /// <summary>
        /// ボス戦の終盤、プレイヤーの左腕を破壊するシーケンス開始に合わせて呼ぶ。
        /// </summary>
        public void BreakLeftArm() => OrderToBoss(EnemyOrder.Type.BreakLeftArm);

        /// <summary>
        /// BreakLeftArmメソッド呼び出し後、1回目のQTEを行う。
        /// </summary>
        public void BossFirstQte() => OrderToBoss(EnemyOrder.Type.BossFirstQTE);

        /// <summary>
        /// ボス戦の終盤、プレイヤーの左腕を破壊するシーケンス開始に合わせて呼ぶ。
        /// </summary>
        public void BossSecondQte() => OrderToBoss(EnemyOrder.Type.BossSecondQTE);

        /// <summary>
        /// ボス戦終了。
        /// </summary>
        public void BossEnd() => OrderToBoss(EnemyOrder.Type.BossEnd);

        /// <summary>
        /// シーケンスを指定してNPCのイベントを実行。
        /// </summary>
        public void PlayNpcEvent(Sequence sequence)
        {
            foreach (INpc npc in NpcInSequecne(sequence))
            {
                npc.Play();
            }
        }

        // シーケンスと種類を指定して敵に命令。
        private void OrderToEnemies(Sequence sequence, EnemyOrder.Type type)
        {
            _order.OrderType = type;

            foreach (EnemyController e in EnemiesInSequence(sequence))
            {
                e.Order(_order);
            }
        }

        // ボスに命令。
        private void OrderToBoss(EnemyOrder.Type type)
        {
            if (_boss == null) return;

            _order.OrderType = type;
            _boss.Order(_order);
        }

        // シーケンス毎の敵を取得。
        private IEnumerable<EnemyController> EnemiesInSequence(Sequence sequence)
        {
            return _enemies.Where(a => a.Params.Sequence == sequence);
        }

        // シーケンス毎のNPCを取得。
        private IEnumerable<INpc> NpcInSequecne(Sequence sequence)
        {
            return _npcs.Where(a => a.Sequence == sequence);
        }

        /// <summary>
        /// 登録する。
        /// </summary>
        public static void Register<T>(T character) => RegisterOrRelease(character, isRegister: true);

        /// <summary>
        /// 登録を解除する。
        /// </summary>
        public static void Release<T>(T character) => RegisterOrRelease(character, isRegister: false);

        // 登録/登録解除
        private static void RegisterOrRelease<T>(T character, bool isRegister)
        {
            GameObject g = GameObject.FindGameObjectWithTag(Const.EnemySystemTag);
            EnemyManager em = g.GetComponent<EnemyManager>();

            if (typeof(T) == typeof(EnemyController))
            {
                if (isRegister) em._enemies.Add(character as EnemyController);
                else em._enemies.Remove(character as EnemyController);
            }
            else if (typeof(T) == typeof(BossController))
            {
                if (isRegister) em._boss = character as BossController;
                else em._boss = null;
            }
            else if (typeof(T) == typeof(INpc))
            {
                if (isRegister) em._npcs.Add(character as INpc);
                else em._npcs.Remove(character as INpc);
            }
        }
    }
}