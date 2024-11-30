using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Enemy.Boss;
using Enemy.NPC;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Enemy
{
    // 敵の行動を管理する
    public class EnemyManager : MonoBehaviour
    {
        // 登録されたボス。
        private BossController _boss;
        // 登録された敵。
        private HashSet<EnemyController> _enemies = new();
        // 登録されたNPC。
        private HashSet<BuddyNpcController> _npcs = new();
        // 登録された敵に対して命令。
        // 命令の処理を呼び出す度に書き換えて使いまわす。
        private EnemyOrder _order = new();

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
        public bool TryGetEnemies(int id, List<EnemyController> enemies)
        {        
            if (_enemies == null) return false;

            enemies.Clear();
            foreach (EnemyController e in EnemiesInSequence(id))
            {
                enemies.Add(e);
            }

            return enemies.Count > 0;
        }

        // /// <summary>
        // /// シーン上に存在する敵の数を返す。
        // /// 生存中の個体のみ、死亡している場合はカウントされない。
        // /// </summary>
        // public int EnemyCount()
        // {
        //     return _enemies.Where(e => e.BlackBoard.IsAlive).Count();
        // }

        /// <summary>
        /// 引数で指定したシーケンスに登場する敵が全員死亡しているかを判定する。
        /// </summary>
        public bool IsAllDefeated(int id)
        {
            return EnemiesInSequence(id).All(e => !e.BlackBoard.IsAlive);
        }

        /// <summary>
        /// 引数で指定したシーケンスの敵を全て撃破する。
        /// </summary>
        public void DefeatThemAll(int id)
        {
            foreach (EnemyController e in EnemiesInSequence(id))
            {
                // 極大ダメージを与えて体力を0にする。
                e.Damage(int.MaxValue / 2, "");
            }
        }

        /// <summary>
        /// 引数で指定したシーケンスに登場する敵全員に対して
        /// 画面上に出現、プレイヤーに接近するように命令する。
        /// </summary>
        public void Spawn(int id)
        {
            OrderToEnemies(id, EnemyOrder.Type.Spawn);
        }

        /// <summary>
        /// シーケンスを指定してポーズ
        /// </summary>
        public void Pause(int id)
        {
            OrderToEnemies(id, EnemyOrder.Type.Pause);
        }

        /// <summary>
        /// シーケンスを指定してポーズ解除
        /// </summary>
        public void Resume(int id)
        {
            OrderToEnemies(id, EnemyOrder.Type.Resume);
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
        /// プレイヤーの正面まで移動して待機させる。
        /// </summary>
        public void PauseOnPlayerForward() => OrderToBoss(EnemyOrder.Type.PauseOnPlayerForward);

        /// <summary>
        /// PauseOnPlayerForwardメソッドでプレイヤーの正面で待機している状態から、ボスの行動を再開する。
        /// </summary>
        public void ResumeBossAction() => OrderToBoss(EnemyOrder.Type.ResumeBossAction);

        /// <summary>
        /// ボス戦でファンネルのレーザーサイト表示。
        /// </summary>
        public void FunnelLaserSight() => OrderToBoss(EnemyOrder.Type.FunnelLaserSight);

        /// <summary>
        /// ボス戦QTE、刀を振り上げつつプレイヤーの正面に移動。
        /// </summary>
        public async UniTask MoveBossToPlayerFrontAsync(CancellationToken token)
        {
            OrderToBoss(EnemyOrder.Type.MoveToPlayerFront);
            await UniTask.WaitUntil(() => _boss.BlackBoard.IsStandingOnQtePosition, cancellationToken: token);
        }

        /// <summary>
        /// ボス戦QTE演出のみ、刀を振り下ろし、プレイヤーの左腕を破壊する。
        /// </summary>
        public void BreakLeftArm() => OrderToBoss(EnemyOrder.Type.BreakLeftArm);

        /// <summary>
        /// ボス戦QTE、プレイヤーの左腕を破壊したモーションから、1回目の鍔迫り合いのために刀を振り上げる。
        /// </summary>
        public void QteCombatReady() => OrderToBoss(EnemyOrder.Type.QteCombatReady);

        /// <summary>
        /// ボス戦QTE、1回目の鍔迫り合い、振り下ろす->弾かれる->再度突っ込んでくる。
        /// </summary>
        public void FirstQteCombatAction() => OrderToBoss(EnemyOrder.Type.FirstQteCombatAction);

        /// <summary>
        /// ボス戦QTE、2回目の鍔迫り合い、振り下ろす->弾かれる->再度突っ込んでくる。
        /// </summary>
        public void SecondQteCombatAction() => OrderToBoss(EnemyOrder.Type.SecondQteCombatAction);

        /// <summary>
        /// ボス戦QTE、パイルバンカーでボスが貫かれる。
        /// </summary>
        public void PenetrateBoss() => OrderToBoss(EnemyOrder.Type.PenetrateBoss);

        /// <summary>
        /// シーケンスを指定してNPCのイベントを実行。
        /// </summary>
        public void PlayNpcEvent(int id)
        {
            foreach (BuddyNpcController npc in NpcInSequecne(id))
            {
                npc.Play();
            }
        }

        // シーケンスと種類を指定して敵に命令。
        private void OrderToEnemies(int id, EnemyOrder.Type type)
        {
            _order.OrderType = type;

            foreach (EnemyController e in EnemiesInSequence(id))
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
        private IEnumerable<EnemyController> EnemiesInSequence(int id)
        {
            return _enemies.Where(a => a.Params.SequenceID == id);
        }

        // シーケンス毎のNPCを取得。
        private IEnumerable<BuddyNpcController> NpcInSequecne(int id)
        {
            return _npcs.Where(a => a.Params.SequenceID == id);
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
            if (g == null || !g.TryGetComponent(out EnemyManager em)) return;

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
            else if (typeof(T) == typeof(BuddyNpcController))
            {
                if (isRegister) em._npcs.Add(character as BuddyNpcController);
                else em._npcs.Remove(character as BuddyNpcController);
            }
        }
    }
}